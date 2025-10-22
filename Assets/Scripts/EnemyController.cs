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

        public Unit Unit { get; private set; }
        private MoveController m_moveController;

        private void Start()
        {
            var agent = GetComponent<NavMeshAgent>();
            var sensor = GetComponent<EnemySensor>();
            m_moveController = new MoveController(agent);
            Unit = new Unit("Enemy", m_moveController, sensor, 0, 100, Unit.Type.Enemy);
            Unit.HealthDidChange += Unit_HealthDidChange;
            m_unitsManager.AddEnemyUnit(Unit);
            m_enemyMachine = Unit.Machine;
            m_enemyMachine.Start();
        }

        private void Unit_HealthDidChange(float healthPoints)
        {
            m_healthBar.UpdateHealthBar(healthPoints, Unit.MaxHealthPoints);
        }
        
        private void OnDisable()
        {
            Unit.HealthDidChange -= Unit_HealthDidChange;
            m_unitsManager.RemoveEnemyUnit(Unit);
            m_enemyMachine.Stop();
        }

        // Update is called once per frame
        void Update()
        {
            m_enemyMachine.Update();
        }
    }
}