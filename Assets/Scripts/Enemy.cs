using System;
using UnityEngine;
using Ezhtellar.AI;
using UnityEngine.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] Transform m_target;
        private StateMachine m_enemyMachine;
        private NavMeshAgent m_agent;

        private string m_lastActivePath = "";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            m_agent = GetComponent<NavMeshAgent>();
            m_enemyMachine.Start();
            m_lastActivePath = m_enemyMachine.PrintActivePath();
            Debug.Log(m_lastActivePath);
        }

        // Update is called once per frame
        void Update()
        {
            m_enemyMachine.Update();
            var path = m_enemyMachine.PrintActivePath();
            if (path != m_lastActivePath)
            {
                m_lastActivePath = path;
                Debug.Log(path);
            }
        }

        public void BuildPlayerMachine()
        {
            m_enemyMachine = StateMachine.FromState(new State.Builder()
                .WithName("Enemy")
                .WithOnEnter(() => Debug.Log("Enemy Machine Started"))
                .WithOnExit(() => Debug.Log("Enemy Machine Stopped"))
                .Build());

            var aliveMachine = StateMachine.FromState(new State.Builder()
                .WithName("Alive")
                .Build());

            var dead = new State.Builder()
                .WithName("Dead")
                .Build();

            m_enemyMachine.AddState(aliveMachine, isInitial: true);
            m_enemyMachine.AddState(dead);

            var idle = new State.Builder()
                .WithName("Idle")
                .Build();

            var movingToLocation = new State.Builder()
                .WithName("MovingToLocation")
                .WithOnEnter(() => { m_agent.SetDestination(m_target.position); })
                .WithOnExit(() => { m_target = null; })
                .Build();

            idle.AddTransition(new Transition(movingToLocation, () => m_target));

            movingToLocation.AddTransition(new Transition(idle,
                () =>
                {
                    return Vector3.Distance(m_target.position, m_agent.transform.position) <= m_agent.stoppingDistance;
                }));

            var attacking = new State.Builder()
                .WithName("Attacking")
                .Build();

            aliveMachine.AddState(idle, isInitial: true);
            aliveMachine.AddState(movingToLocation);
            aliveMachine.AddState(attacking);
        }
    }
}