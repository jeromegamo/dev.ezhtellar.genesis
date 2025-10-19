using UnityEngine;
using UnityEngine.AI;

namespace Ezhtellar.Genesis
{
    public class MoveController
    {
        private NavMeshAgent m_agent;
        
        public Vector3 Position => m_agent.transform.position;
        public bool HasReachedDestination
        {
            get
            {
                return m_agent.remainingDistance <= m_agent.stoppingDistance && 
                       m_agent.velocity.sqrMagnitude < 0.01f;
            }
        }

        public bool HasDestination => m_agent.hasPath;
        
        public MoveController(NavMeshAgent agent)
        {
            m_agent = agent;
        }
        
        public void MoveTo(Vector3 destination, float stoppingDistance)
        {
            m_agent.stoppingDistance = stoppingDistance;
            m_agent.SetDestination(destination);
        }

        public void StopMoving()
        {
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