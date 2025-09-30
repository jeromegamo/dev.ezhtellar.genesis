using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class UnitsManager
    {
        private List<IUnit> m_playableUnits;
        private List<IUnit> m_selectedUnits;

        public IEnumerable<IUnit> PlayableUnits => m_playableUnits;
        public IEnumerable<IUnit> SelectedUnits => m_selectedUnits;

        public static int MaxPlayableUnits => 4;

        public UnitsManager(IEnumerable<IUnit> playableUnits)
        {
            m_playableUnits = playableUnits.ToList();
            m_selectedUnits = new List<IUnit>();
            m_playableUnits.ForEach(unit =>
            {
                unit.Selected += AddSelectedUnit;
                unit.Deselected += RemoveSelectedUnit;
            });
        }

        ~UnitsManager()
        {
            m_playableUnits.ForEach(unit =>
            {
                unit.Selected -= AddSelectedUnit;
                unit.Deselected -= RemoveSelectedUnit;
            });
        }

        private void AddSelectedUnit(IUnit u) => m_selectedUnits.Add(u);

        private void RemoveSelectedUnit(IUnit u) => m_selectedUnits.Remove(u);

        public void DeselectAllUnits()
        {
            for (int i = m_selectedUnits.Count() - 1; i >= 0; i--)
            {
                m_selectedUnits[i].Deselect();
            }
        }
    }
}