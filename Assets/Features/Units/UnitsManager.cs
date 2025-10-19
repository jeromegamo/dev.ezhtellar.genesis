using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class UnitsManager
    {
        private List<Unit> m_playableUnits;
        private List<Unit> m_selectedUnits;
        private List<Unit> m_enemyUnits;

        public IEnumerable<Unit> PlayableUnits => m_playableUnits;
        public IEnumerable<Unit> SelectedUnits => m_selectedUnits;
        public List<Unit> EnemyUnits => m_enemyUnits;

        public static int MaxPlayableUnits => 4;

        public UnitsManager()
        {
            m_playableUnits = new List<Unit>();
            m_selectedUnits = new List<Unit>();
            m_enemyUnits = new List<Unit>();
        }

        public void AddSelectedUnit(Unit u) => m_selectedUnits.Add(u);

        public void RemoveSelectedUnit(Unit u) => m_selectedUnits.Remove(u);
        
        public void AddPlayableUnit(Unit u) => m_playableUnits.Add(u);
        public void RemovePlayableUnit(Unit u) => m_playableUnits.Remove(u);
        
        public void AddEnemyUnit(Unit u) => m_enemyUnits.Add(u);
        public void RemoveEnemyUnit(Unit u) => m_enemyUnits.Remove(u);

        public void DeselectAllUnits()
        {
            for (int i = m_selectedUnits.Count() - 1; i >= 0; i--)
            {
                m_selectedUnits[i].Deselect();
            }
        }
    }
}