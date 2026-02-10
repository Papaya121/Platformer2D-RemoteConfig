using UnityEngine;

public abstract class HealthController : MonoBehaviour, IHealth
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        OnDamage();
    }

    protected virtual void OnDamage(){}
    protected abstract void Die();
}