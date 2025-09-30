using System;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IUnit
    {
        event Action<IUnit> Selected;
        event Action<IUnit> Deselected;

        int FormationSlotNumber { get; }
        Vector3 Position { get; }

        void Select();
        void Deselect();
        void Move(Vector3 direction);
    }
}