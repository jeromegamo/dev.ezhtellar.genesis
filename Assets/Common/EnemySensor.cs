using System;
using Ezhtellar.Genesis;
using Reflex.Attributes;
using UnityEngine;

namespace Common
{
    public class EnemySensor: MonoBehaviour, IEnemySensor
    {
        [Inject] private UnitsManager m_unitsManager;
        
        private Unit m_unitHost;
        private float m_detectionRange = 0;
        private bool m_shouldDetect = false;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_detectionRange);
        }

        public event Action<Unit> DidDetectEnemy;
        
        public void SetUnitHost(Unit unitHost)
        {
            m_unitHost = unitHost;
            m_detectionRange =  m_unitHost.DetectionRange;
        }

        public void StartDetecting()
        {
           m_shouldDetect = true; 
        }

        public void StopDetecting()
        {
           m_shouldDetect = false; 
        }

        private void Update()
        {
            if (!m_shouldDetect) { return;}
            if (m_unitHost == null) { return; }
            
            switch (m_unitHost.UnitType)
            {
                case Unit.Type.Enemy:
                {
                    foreach (var playableUnit in m_unitsManager.PlayableUnits)
                    {
                        IsInRange(playableUnit); 
                    }

                    break;
                }
                case Unit.Type.Playable:
                {
                    foreach (var enemyUnit in m_unitsManager.EnemyUnits)
                    {
                        IsInRange(enemyUnit); 
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void IsInRange(Unit unit)
        {
            if (Vector3.Distance(m_unitHost.Position, unit.Position) < m_unitHost.DetectionRange)
            {
                DidDetectEnemy?.Invoke(unit);
            }
        }
    }
}