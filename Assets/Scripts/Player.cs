using System;
using UnityEngine;
using Ezhtellar.AI;
using UnityEngine.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform m_target;
        private StateMachine m_playerMachine;
        private NavMeshAgent m_agent;

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
        }

        // Update is called once per frame
        void Update()
        {
            m_playerMachine.Update();
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