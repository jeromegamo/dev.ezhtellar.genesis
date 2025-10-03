using UnityEngine;

namespace Ezhtellar.Genesis
{
    public interface IDamageable
    {
        public Vector3 Position { get; }
        public void TakeDamage(float damage);
        public float Health { get; }
    }

    public class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health = 100;

        public Vector3 Position => transform.position;
        
        public float Health => health;

        public void TakeDamage(float damage)
        {
            Debug.Log($"taking damage {damage}");
            health = Mathf.Clamp(health - damage, 0f, health);
        }
    }
}