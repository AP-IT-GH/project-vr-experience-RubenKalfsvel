using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    public float currentHealth { get; private set; } 


    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= amount;
        Debug.Log($"{transform.name} took {amount} damage. Current Health: {currentHealth}");



        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{transform.name} has died.");

    }


    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        Debug.Log($"{transform.name} has been reset. Health: {currentHealth}");

    }

    
}

