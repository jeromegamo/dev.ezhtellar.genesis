using System;
using System.Linq;
using Common;
using UnityEngine;
using Ezhtellar.AI;
using Reflex.Attributes;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(DetectionRadius))]
    [RequireComponent(typeof(IDamageable))]
    [RequireComponent(typeof(IMoveable))]
    public class Enemy : MonoBehaviour
    {
        [Inject] UnitsManager m_unitsManager;
        [SerializeField] float m_detectionRadius = 5f;
        [SerializeField] float m_attackDamage = 5f;
        private StateMachine m_enemyMachine;
        IUnit m_targetUnit;
        private IMoveable m_moveable;
        IDamageable m_ownDamageable;
        float m_moveToLocStopDistance = 1;
        float m_moveToUnitStopDistance = 2;
        float m_attackRange = 2.5f;
        float m_timeBetweenAttacks = 1f;
        float m_sinceLastAttack = Mathf.Infinity;
        private string m_lastActivePath = "";


        void OnEnable()
        {
            BuildPlayerMachine();
        }

        void OnDisable()
        {
            m_enemyMachine.Stop();
        }

        private void Start()
        {
            m_moveable = GetComponent<IMoveable>();
            m_ownDamageable = GetComponent<IDamageable>();
            m_enemyMachine.Start();
        }

        // Update is called once per frame
        void Update()
        {
            m_enemyMachine.Update();
            
            Debug.Log($"reached: {m_moveable.HasReachedDestination}");
            var path = m_enemyMachine.PrintActivePath();
            if (m_lastActivePath != path)
            {
                Debug.Log(path);
                m_lastActivePath = path;
            }
        }

        public void BuildPlayerMachine()
        {
            m_enemyMachine = StateMachine.FromState(new State.Builder()
                .WithName("Enemy")
                .WithOnEnter(() => Debug.Log("Enemy Machine Started"))
                .WithOnExit(() => Debug.Log("Enemy Machine Stopped"))
                .Build());
            
            var alive = new State.Builder()
                .WithName("Alive")
                .Build();
            
            var aliveMachine = StateMachine.FromState(alive);

            var dead = new State.Builder()
                .WithName("Dead")
                .WithOnEnter(() => gameObject.SetActive(false))
                .Build();

            var idle = new State.Builder()
                .WithName("Idle")
                .WithOnEnter(() => m_targetUnit = null)
                .WithOnUpdate(() =>
                {
                })
                .Build();

            var movingToLocation = new State.Builder()
                .WithName("MovingToLocation")
                .WithOnUpdate(() =>
                {
                    if (m_targetUnit != null)
                    {
                        m_moveable.MoveTo(m_targetUnit.Position, m_attackRange);
                    }
                })
                .WithOnExit(() => m_moveable.StopMoving())
                .Build();

            var attacking = new State.Builder()
                .WithName("Attacking")
                .WithOnEnter(() => m_moveable.StopMoving())
                .WithOnUpdate(() =>
                {
                    m_sinceLastAttack += Time.deltaTime;
                    if (m_sinceLastAttack > m_timeBetweenAttacks)
                    {
                        m_targetUnit?.TakeDamage(m_attackDamage);
                        m_sinceLastAttack = 0;
                    } 
                })
                .Build();

            aliveMachine.AddState(idle, isInitial: true);
            aliveMachine.AddState(movingToLocation);
            aliveMachine.AddState(attacking);
            
            idle.AddTransition(new Transition(movingToLocation, () =>
            {
                // we already have a target don't find a new one
                if (m_targetUnit != null) return false;
                
                foreach (var unit in m_unitsManager.PlayableUnits)
                {
                    if ((Vector3.Distance(unit.Position, transform.position) <= m_detectionRadius) &&
                        !unit.IsDead)
                    {
                        m_targetUnit = unit;
                        return true;
                    }
                }
                
                return false;
            }));
            
            movingToLocation.AddTransition(new Transition(attacking, () => 
                m_targetUnit != null && Vector3.Distance(transform.position, m_targetUnit.Position) < m_attackRange));
            movingToLocation.AddTransition(new Transition(idle, () => 
                Vector3.Distance(transform.position, m_targetUnit.Position) > m_detectionRadius));
            attacking.AddTransition(new Transition(movingToLocation, () => 
                m_targetUnit != null && 
                Vector3.Distance(transform.position, m_targetUnit.Position) > m_attackRange &&
                Vector3.Distance(transform.position, m_targetUnit.Position) < m_detectionRadius));
            attacking.AddTransition(new Transition(idle, () => m_targetUnit != null && m_targetUnit.IsDead));
            
            alive.AddTransition(new Transition(dead, () => m_ownDamageable.Health <= 0));
            
            m_enemyMachine.AddState(aliveMachine, isInitial: true);
            m_enemyMachine.AddState(dead);


        }
    }
}