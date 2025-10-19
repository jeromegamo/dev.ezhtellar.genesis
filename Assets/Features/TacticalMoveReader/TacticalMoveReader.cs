using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ezhtellar.Genesis
{
    public class TacticalMoveReader
    {
        private IFormation m_formation;
        private UnitsManager m_unitManager;
        private IInteractionReader m_interactionReader;

        public TacticalMoveReader(UnitsManager unitsManager, IInteractionReader interactionReader,
            IFormation defaultFormation)
        {
            m_unitManager = unitsManager;
            m_interactionReader = interactionReader;
            m_formation = defaultFormation;
            m_interactionReader.WillMove += InteractionReader_WillMove;
            m_interactionReader.WillSetFormation += InteractionReader_WillSetFormation;
            m_interactionReader.RotatingFormation += InteractionReader_RotatingFormation;
        }

        ~TacticalMoveReader()
        {
            m_interactionReader.WillMove -= InteractionReader_WillMove;
            m_interactionReader.WillSetFormation -= InteractionReader_WillSetFormation;
            m_interactionReader.RotatingFormation -= InteractionReader_RotatingFormation;
        }

        private void InteractionReader_RotatingFormation(Vector3 formationRotation)
        {
            m_formation.SetRotation(formationRotation);
        }

        private void InteractionReader_WillSetFormation(Vector3 formationLocation)
        {
            m_formation.HideAllSlots();
            m_formation.SetPosition(formationLocation);
            int selectedUnitsCount = m_unitManager.SelectedUnits.Count();
            m_formation.ShowSlots(selectedUnitsCount);
        }

        private void InteractionReader_WillMove(Vector3 destination)
        {
            MoveSelectedUnits();
        }

        private void MoveSelectedUnits()
        {
            var slots = m_formation.GetSlotPositions();

            IOrderedEnumerable<Unit> orderedSelectedUnits = m_unitManager.SelectedUnits
                .OrderBy(unit => unit.FormationSlotNumber);

            if (UnitsManager.MaxPlayableUnits == m_unitManager.SelectedUnits.Count())
            {
                foreach (Unit unit in orderedSelectedUnits)
                {
                    IFormation.SlotPosition? slotPosition = slots
                        .First(slot => slot.SlotIndex == unit.FormationSlotNumber);

                    if (!slotPosition.HasValue)
                    {
                        throw new Exception($"No slot {unit.FormationSlotNumber} was found for the selected unit");
                    }

                    unit.Move(slotPosition.Value.Position);
                }
            }
            else
            {
                IEnumerable<(Unit unit, IFormation.SlotPosition slot)> zipped =
                    orderedSelectedUnits.Zip(slots, (unit, slot) => (unit, slot));
                foreach ((Unit unit, IFormation.SlotPosition slot) pair in zipped)
                {
                    pair.unit.Move(pair.slot.Position);
                }
            }
        }
    }
}