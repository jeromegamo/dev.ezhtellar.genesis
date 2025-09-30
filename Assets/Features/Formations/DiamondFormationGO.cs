using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SlotPosition = Ezhtellar.Genesis.IFormation.SlotPosition;

namespace Ezhtellar.Genesis
{
    public class DiamondFormationGO : MonoBehaviour, IFormation
    {
        [SerializeField] private List<Transform> slots;

        private void Start()
        {
            this.gameObject.SetActive(true);
            HideAllSlots();
        }

        public static DiamondFormationGO Instantiate()
        {
            DiamondFormationGO diamondFormationGo = Resources.Load<DiamondFormationGO>("DiamondFormation");
            diamondFormationGo.gameObject.SetActive(false);
            DiamondFormationGO go = Instantiate(diamondFormationGo);
            go.gameObject.SetActive(true);
            return go;
        }

        public IEnumerable<SlotPosition> GetSlotPositions() =>
            slots
                .Where(slot => slot.gameObject.activeInHierarchy)
                .Select((t, index) => { return new SlotPosition { SlotIndex = index + 1, Position = t.position }; });

        public void ShowSlots(int moveableUnitsCount)
        {
            foreach (var slot in slots
                         .Take(moveableUnitsCount))
            {
                slot.gameObject.SetActive(true);
            }
        }

        public void HideAllSlots() =>
            slots.ForEach(slot => slot.gameObject.SetActive(false));

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            transform.Rotate(rotation);
        }
    }
}