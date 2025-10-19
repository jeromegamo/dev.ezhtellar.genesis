using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    private float maxHealth;
    private float currentHealth;


    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.rectTransform.localScale = new Vector3(x: currentHealth / maxHealth, y: 1, z: 1);
    }
}
