using System;
using Ezhtellar.AI;
using JetBrains.Annotations;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class Unit
    {
        public enum Type
        {
            Playable,
            Enemy
        }

        private Unit m_target;
        private MoveController m_moveController;
        private IEnemySensor m_enemySensor;
        private float m_health;
        private float m_maxHealthPoints;
        private float m_attackRange = 2.5f;
        private float m_detectionRange = 10f;
        private float m_stoppingDistance = 0;
        private float m_sinceLastAttack = Mathf.Infinity;
        private float m_sinceLastPlayerControlled = Mathf.Infinity;
        private float m_timeAllowedForAIIdle = 5f;
        private float m_attackDamage = 10;
        private float m_timeBetweenAttacks = 1f;
        private bool m_isPlayerControlled = false;

        public StateMachine Machine { get; private set; }
        public bool IsSelected { get; private set; }
        public Unit.Type UnitType { get; }
        public string UnitName { get; }
        public float DetectionRange => m_detectionRange;

        public float HealthPoints
        {
            get => m_health;
            private set
            {
                m_health = Mathf.Clamp(value, 0, m_maxHealthPoints);
                HealthDidChange?.Invoke(m_health);
            }
        }

        public float MaxHealthPoints => m_maxHealthPoints;
        public Vector3 Position => m_moveController.Position;
        public bool IsDead => m_health <= 0;
        public int FormationSlotNumber { get; private set; }

        public event Action<Unit> DidSelect;
        public event Action<Unit> DidDeselect;
        public event Action<float> HealthDidChange;

        public Unit(
            string unitName,
            MoveController moveController,
            IEnemySensor enemySensor,
            int formationSlotNumber,
            float maxHealthPoints,
            Unit.Type unitType)
        {
            UnitName = unitName;
            m_maxHealthPoints = maxHealthPoints;
            m_health = maxHealthPoints;
            m_moveController = moveController;
            m_enemySensor = enemySensor;
            m_enemySensor.DidDetectEnemy += EnemySensor_DidDetectEnemy;
            FormationSlotNumber = formationSlotNumber;
            UnitType = unitType;
            m_enemySensor.SetUnitHost(this);
            BuildStateMachine();
        }

        public void Select()
        {
            IsSelected = true;
            DidSelect?.Invoke(this);
        }

        public void Deselect()
        {
            IsSelected = false;
            DidDeselect?.Invoke(this);
        }

        public void Move(Vector3 destination)
        {
            m_isPlayerControlled = true;
            m_target = null;
            m_enemySensor.StopDetecting();
            m_sinceLastPlayerControlled = 0;
            m_stoppingDistance = 0;
            m_moveController.MoveTo(destination, m_stoppingDistance);
        }

        public void SetTarget(Unit target)
        {
            m_isPlayerControlled = true;
            m_enemySensor.StopDetecting();
            m_sinceLastPlayerControlled = 0;
            m_target = target;
            m_stoppingDistance = m_attackRange;
            m_moveController.MoveTo(m_target.Position, m_stoppingDistance);
        }

        public void TakeDamage(float damage)
        {
            HealthPoints -= damage;
            HealthDidChange?.Invoke(HealthPoints);
        }

        private void AutoSetTarget(Unit target)
        {
            m_isPlayerControlled = false;
            m_target = target;
            m_stoppingDistance = m_attackRange;
            m_moveController.MoveTo(m_target.Position, m_stoppingDistance);
        }

        private void BuildStateMachine()
        {
            Machine = StateMachine.FromState(
                new State.Builder()
                    .WithName("Root")
                    .WithOnUpdate(() =>
                        {
                            if (!m_isPlayerControlled)
                            {
                                m_enemySensor.StartDetecting();
                            }

                            m_sinceLastPlayerControlled += Time.deltaTime;
                            if (m_sinceLastPlayerControlled > m_timeAllowedForAIIdle)
                            {
                                m_isPlayerControlled = false;
                            }
                        }
                    )
                    .Build()
            );

            var aliveMachine = StateMachine.FromState(
                new State.Builder()
                    .WithName("Alive")
                    .Build()
            );

            var dead = new State.Builder()
                .WithName("Dead")
                .Build();

            Machine.AddState(aliveMachine, isInitial: true);
            Machine.AddState(dead);

            BuildAliveMachine(aliveMachine);
        }

        private void BuildAliveMachine(StateMachine aliveMachine)
        {
            var idle = new State.Builder()
                .WithName("Idle")
                .Build();

            var movingToLocation = new State.Builder()
                .WithName("MovingToLocation")
                .WithOnUpdate(() =>
                    {
                        if (m_target != null)
                        {
                            m_moveController.MoveTo(m_target.Position, m_stoppingDistance);
                        }
                    }
                )
                .Build();


            var attacking = new State.Builder()
                .WithName("Attacking")
                .WithOnUpdate(() =>
                    {
                        m_sinceLastAttack += Time.deltaTime;
                        if (m_sinceLastAttack > m_timeBetweenAttacks)
                        {
                            m_target?.TakeDamage(m_attackDamage);
                            m_sinceLastAttack = 0;
                        }
                    }
                )
                .Build();


            idle.AddTransition(
                new Transition(
                    movingToLocation,
                    () => m_moveController.HasDestination && !m_moveController.HasReachedDestination
                )
            );

            idle.AddTransition(
                new Transition(
                    attacking,
                    () => m_target != null &&
                          Vector3.Distance(Position, m_target.Position) <= m_attackRange
                )
            );

            movingToLocation.AddTransition(
                new Transition(
                    idle,
                    () =>
                        m_target != null && Vector3.Distance(Position, m_target.Position) > m_detectionRange
                )
            );

            movingToLocation.AddTransition(
                new Transition(
                    attacking,
                    () => m_target != null &&
                          Vector3.Distance(Position, m_target.Position) <= m_attackRange
                )
            );

            attacking.AddTransition(
                new Transition(
                    idle,
                    () => m_target != null &&
                          (Vector3.Distance(Position, m_target.Position) > m_attackRange ||
                          m_target.IsDead)
                )
            );


            aliveMachine.AddState(idle, isInitial: true);
            aliveMachine.AddState(movingToLocation);
            aliveMachine.AddState(attacking);
        }

        private void EnemySensor_DidDetectEnemy(Unit unit)
        {
            AutoSetTarget(unit);
        }
    }
}