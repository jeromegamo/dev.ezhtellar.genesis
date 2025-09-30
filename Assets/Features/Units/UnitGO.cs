using System;
using UnityEngine;
using UnityEngine.AI;
using Ezhtellar.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitGO : MonoBehaviour, IUnit
    {
        [SerializeField] private GameObject m_selectionDecal;
        [SerializeField] private NavMeshAgent m_agent;
        [SerializeField] private int m_formationSlotNumber;

        StateMachine m_playerMachine;
        Vector3? m_targetMoveLocation;
        IDamageable m_targetUnit;
        float m_attackRange = 2.5f;
        float m_timeBetweenAttacks = 1f;
        float m_sinceLastAttack = Mathf.Infinity;
        float m_attackDamage = 50; 
        string m_lastActivePath = "";
        public Vector3 Position => transform.position;
        public int FormationSlotNumber => m_formationSlotNumber;

        public event Action<IUnit> Selected;
        public event Action<IUnit> Deselected;

        private void Awake() => m_selectionDecal.SetActive(false);

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
            m_agent = GetComponent<NavMeshAgent>();
            m_playerMachine.Start();
            
            m_lastActivePath = m_playerMachine.PrintActivePath();
            Debug.Log(m_lastActivePath);
        }

        // Update is called once per frame
        void Update()
        {
            m_playerMachine.Update();
            var path = m_playerMachine.PrintActivePath();
            if (m_lastActivePath != path)
            {
                Debug.Log(path);
                m_lastActivePath = path;
            }
        }

        public void Move(Vector3 destination)
        {
            m_targetMoveLocation = destination;
            m_agent.stoppingDistance = 1;
        }

        public void SetTarget(IDamageable target)
        {
            m_targetUnit = target;
            m_targetMoveLocation = target.Position;
            m_agent.stoppingDistance = m_attackRange;
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
                .Build();

            var movingToLocation = new State.Builder()
                .WithName("MovingToLocation")
                .WithOnUpdate(() =>
                {
                    if (m_targetMoveLocation.HasValue)
                    {
                        m_agent.SetDestination(m_targetMoveLocation.Value);
                    }
                })
                .WithOnExit(() => m_targetMoveLocation = null)
                .Build();

            // Note: Stopping distance should not be set to 0 as the distance calculation will never reach 0.
            movingToLocation.AddTransition(new Transition(idle,
                () => m_targetMoveLocation.HasValue && 
                      Vector3.Distance(m_targetMoveLocation.Value, m_agent.transform.position) <= m_agent.stoppingDistance));
            
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
            
            idle.AddTransition(new Transition(movingToLocation, () => m_targetMoveLocation.HasValue));
            idle.AddTransition(new Transition(attacking, () => m_targetUnit != null &&
               (Vector3.Distance(m_targetUnit.Position, m_agent.transform.position) <= m_agent.stoppingDistance)));
            


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