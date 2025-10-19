using System;
using Common;
using UnityEngine;
using UnityEngine.AI;
using Ezhtellar.AI;
using Reflex.Attributes;

namespace Ezhtellar.Genesis
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemySensor))]
    public class UnitController : MonoBehaviour
    {
        [Inject] private UnitsManager m_unitsManager;
        [SerializeField] private StateMachine m_playerMachine;
        [SerializeField] private int m_formationSlotNumber;
        [SerializeField] private GameObject m_selectionDecal;
        [SerializeField] private float healthPoints = 100;
        [SerializeField] private float maxHealthPoints = 100;
        [SerializeField] HealthBar m_healthBar;
        
        public Unit Unit { get; private set; }

        private void Start()
        {
            var agent = GetComponent<NavMeshAgent>();
            var sensor = GetComponent<EnemySensor>();
            var moveController = new MoveController(agent);
            Unit = new Unit("Player", moveController, sensor, m_formationSlotNumber, maxHealthPoints, Unit.Type.Playable);
            Unit.DidSelect += Unit_DidSelect;
            Unit.DidDeselect += Unit_DidDeselect;
            Unit.HealthDidChange += Unit_HealthDidChange;
            m_unitsManager.AddPlayableUnit(Unit);
            
            m_playerMachine = Unit.Machine;
            m_playerMachine.Start();
            
        }

        private void Unit_HealthDidChange(float healthPoints)
        {
            m_healthBar.UpdateHealthBar(healthPoints, Unit.MaxHealthPoints);
        }

        private void OnDisable()
        {
            Unit.DidSelect -= Unit_DidSelect;
            Unit.DidDeselect -= Unit_DidDeselect;
            Unit.HealthDidChange -= Unit_HealthDidChange;
            m_unitsManager.RemovePlayableUnit(Unit);
            m_playerMachine.Stop();
        }
        
        private void Update()
        {
            m_playerMachine.Update();
        }

        private void Unit_DidSelect(Unit unit)
        {
            m_unitsManager.AddSelectedUnit(unit);
            m_selectionDecal.SetActive(true);
        }
        
        private void Unit_DidDeselect(Unit unit)
        {
            m_unitsManager.RemoveSelectedUnit(unit);
            m_selectionDecal.SetActive(false);
        }
    }
}