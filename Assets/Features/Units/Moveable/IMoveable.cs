using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IMoveable
    {
        public bool HasReachedDestination { get; }
        public bool HasDestination { get; }
        public void MoveTo(Vector3 destination, float stoppingDistance);
        public void StopMoving();
        public void PauseMoving();
        public void ResumeMoving();
    }
}