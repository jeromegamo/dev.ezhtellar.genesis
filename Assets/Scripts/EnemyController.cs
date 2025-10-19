using System;
using System.Linq;
using Common;
using UnityEngine;
using Ezhtellar.AI;
using Reflex.Attributes;
using UnityEngine.AI;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(EnemySensor))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [Inject] UnitsManager m_unitsManager;
        [SerializeField] private StateMachine m_enemyMachine;
        [SerializeField] HealthBar m_healthBar;

        Unit m_unit;
        private MoveController m_moveController;

        private void Start()
        {
            var agent = GetComponent<NavMeshAgent>();
            var sensor = GetComponent<EnemySensor>();
            m_moveController = new MoveController(agent);
            m_unit = new Unit("Enemy", m_moveController, sensor, 0, 100, Unit.Type.Enemy);
            m_unit.HealthDidChange += Unit_HealthDidChange;
            m_unitsManager.AddEnemyUnit(m_unit);
            m_enemyMachine = m_unit.Machine;
            m_enemyMachine.Start();
        }

        private void Unit_HealthDidChange(float healthPoints)
        {
            m_healthBar.UpdateHealthBar(healthPoints, m_unit.MaxHealthPoints);
        }
        
        private void OnDisable()
        {
            m_unit.HealthDidChange -= Unit_HealthDidChange;
            m_unitsManager.RemoveEnemyUnit(m_unit);
            m_enemyMachine.Stop();
        }

        // Update is called once per frame
        void Update()
        {
            m_enemyMachine.Update();
        }
    }
}