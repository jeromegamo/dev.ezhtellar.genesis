using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IDamageable
    {
        public Vector3 Position { get; }
        public void TakeDamage(float damage);
    }

    public class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health = 100;

        public Vector3 Position => transform.position;

        public void TakeDamage(float damage)
        {
            health = Mathf.Clamp(health - damage, 0f, health);
        }
    }
}