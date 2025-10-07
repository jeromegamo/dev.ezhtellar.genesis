using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IDamageable
    {
        public Vector3 Position { get; }
        public float Health { get; }
        public bool IsDead { get; }
        public void TakeDamage(float damage);
    }
}