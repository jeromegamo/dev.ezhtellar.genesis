using System;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IInteractionReader
    {
        public event Action<Unit> WillAttack;
        public event Action<Vector3> WillMove;
        public event Action<Vector3> WillSetFormation;
        public event Action<Vector3> RotatingFormation;
    }
}