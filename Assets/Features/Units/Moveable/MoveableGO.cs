using System;
using UnityEngine;
using UnityEngine.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MoveableGO : MonoBehaviour, IMoveable
    {
        [SerializeField] float m_moveSpeed = 5;

        NavMeshAgent m_agent;
        
        public bool HasReachedDestination => !m_agent.hasPath && m_agent.remainingDistance == 0;
        public bool HasDestination => m_agent.hasPath;

        private void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_agent.speed = m_moveSpeed;
        }
        

        public void MoveTo(Vector3 destination, float stoppingDistance)
        {
            m_agent.isStopped = false;
            m_agent.stoppingDistance = stoppingDistance;
            m_agent.SetDestination(destination);
        }

        public void StopMoving()
        {
            m_agent.isStopped = true;
            m_agent.ResetPath();
        }

        public void PauseMoving()
        {
            m_agent.isStopped = true;
        }

        public void ResumeMoving()
        {
            m_agent.isStopped = false;
        }
    }
}