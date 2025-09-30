using System.Collections.Generic;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IFormation
    {
        public IEnumerable<SlotPosition> GetSlotPositions();
        public void ShowSlots(int moveableUnitsCount);
        public void HideAllSlots();
        public void SetPosition(Vector3 position);
        public void SetRotation(Vector3 rotation);

        public struct SlotPosition
        {
            public int SlotIndex;
            public Vector3 Position;
        }
    }
}