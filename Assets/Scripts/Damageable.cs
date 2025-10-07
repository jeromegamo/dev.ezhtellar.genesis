using System;
using UnityEngine;

namespace Ezhtellar.Genesis
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        [SerializeField] private float health = 100;
        [SerializeField] HealthBar m_healthBar;

        public Vector3 Position => transform.position;
        
        public float Health => m_healthBar.CurrentHealth;
        
        public bool IsDead => m_healthBar.CurrentHealth <= 0;

        private void Start()
        {
            m_healthBar.SetMaxHealth(health);
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"taking damage {damage}");
            m_healthBar.DecreaseHealth(damage);
        }
    }
}