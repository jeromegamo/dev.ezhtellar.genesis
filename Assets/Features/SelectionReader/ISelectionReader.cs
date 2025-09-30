using System;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface ISelectionReader
    {
        public event Action<Vector2> WillSelect;
        public event Action<Vector2> DraggingSelection;
        public event Action<Vector2> DidSelect;
    }
}