using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 3f;
    private float currentHealth;
    private bool isInvulnerable;
    public float invulnerabilityDuration = 0.5f;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Player health initialized to {currentHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        SkeletonAgent[] agents = FindObjectsByType<SkeletonAgent>(FindObjectsSortMode.None);
        foreach (SkeletonAgent agent in agents)
        {
            agent.OpponentDied();
        }
    }
}