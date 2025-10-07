using System;
using UnityEngine;
using UnityEngine.AI;
using Ezhtellar.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(IDamageable))]
    [RequireComponent(typeof(IMoveable))]
    public class UnitGO : MonoBehaviour, IUnit
    {
        [SerializeField] private GameObject m_selectionDecal;
        [SerializeField] private int m_formationSlotNumber;

        [SerializeField]
        StateMachine m_playerMachine;
        Vector3? m_targetMoveLocation;
        IDamageable m_ownDamageable;
        IDamageable m_targetUnit;
        float m_attackRange = 2.5f;
        float m_timeBetweenAttacks = 1f;
        float m_sinceLastAttack = Mathf.Infinity;
        float m_attackDamage = 10; 
        string m_lastActivePath = "";
        private float m_currentStoppingDistance = 0;
        IMoveable m_moveable;
        public Vector3 Position => transform.position;
        public bool IsDead => m_ownDamageable.Health <= 0;
        public int FormationSlotNumber => m_formationSlotNumber;

        public event Action<IUnit> Selected;
        public event Action<IUnit> Deselected;

        private void Awake()
        {
            m_selectionDecal.SetActive(false);
        }

        void OnEnable()
        {
            BuildPlayerMachine();
        }

        void OnDisable()
        {
            m_playerMachine.Stop();
        }

        private void Start()
        {
            m_moveable = GetComponent<IMoveable>();
            m_ownDamageable = GetComponent<IDamageable>();
            m_playerMachine.Start();
        }

        // Update is called once per frame
        void Update()
        {
            m_playerMachine.Update();
        }

        public void Move(Vector3 destination)
        {
            m_targetMoveLocation = destination;
            m_currentStoppingDistance = 1;
            m_moveable.MoveTo(destination, m_currentStoppingDistance);
        }

        public void SetTarget(IDamageable target)
        {
            m_targetUnit = target;
            m_targetMoveLocation = target.Position;
            m_currentStoppingDistance = m_attackRange;
            m_moveable.MoveTo(target.Position, m_currentStoppingDistance);
        }

        public void TakeDamage(float damage)
        {
            m_ownDamageable.TakeDamage(damage);
        }

        public void BuildPlayerMachine()
        {
            m_playerMachine = StateMachine.FromState(new State.Builder()
                .WithName("Player")
                .WithOnEnter(() => Debug.Log("Player Machine Started"))
                .WithOnExit(() => Debug.Log("Player Machine Stopped"))
                .Build());

            var aliveMachine = StateMachine.FromState(new State.Builder()
                .WithName("Alive")
                .Build());

            var dead = new State.Builder()
                .WithName("Dead")
                .Build();

            m_playerMachine.AddState(aliveMachine, isInitial: true);
            m_playerMachine.AddState(dead);

            var idle = new State.Builder()
                .WithName("Idle")
                .WithOnEnter(() => { m_targetUnit = null;})
                .Build();

            var movingToLocation = new State.Builder()
                .WithName("MovingToLocation")
                .WithOnUpdate(() =>
                {
                    if (m_targetUnit != null)
                    {
                        m_moveable.MoveTo(m_targetUnit.Position, m_currentStoppingDistance);
                    }
                })
                //.WithOnExit(() => m_moveable.StopMoving())
                .Build();

            
            var attacking = new State.Builder()
                .WithName("Attacking")
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
            
            idle.AddTransition(new Transition(movingToLocation, () => m_moveable.HasDestination));
            idle.AddTransition(new Transition(attacking, () => m_targetUnit != null && m_moveable.HasReachedDestination));
            movingToLocation.AddTransition(new Transition(idle, () => m_moveable.HasReachedDestination));
            movingToLocation.AddTransition(new Transition(attacking, () => 
                m_targetUnit != null && Vector3.Distance(transform.position, m_targetUnit.Position) <= m_attackRange));
            attacking.AddTransition(new Transition(movingToLocation, () => 
                m_targetUnit != null && 
                Vector3.Distance(transform.position, m_targetUnit.Position) > m_attackRange));
            attacking.AddTransition(new Transition(idle, () => m_targetUnit != null && m_targetUnit.IsDead));
            aliveMachine.AddState(idle, isInitial: true);
            aliveMachine.AddState(movingToLocation);
            aliveMachine.AddState(attacking);
        }

        public void Select()
        {
            m_selectionDecal.SetActive(true);
            Selected?.Invoke(this);
        }

        public void Deselect()
        {
            m_selectionDecal.SetActive(false);
            Deselected?.Invoke(this);
        }
    }
}