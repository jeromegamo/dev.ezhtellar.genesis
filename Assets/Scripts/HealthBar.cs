using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    private float maxHealth;
    private float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    
    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar(currentHealth, maxHealth);
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar(currentHealth, maxHealth);
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthBar(currentHealth, maxHealth);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.rectTransform.localScale = new Vector3(x: currentHealth / maxHealth, y: 1, z: 1);
    }
}
