using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    private bool canDealDamage;
    private bool hasDealtDamage;

    [SerializeField] private Transform tip;
    [SerializeField] private Transform basePoint;
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float targetCooldown = 0.5f;

    private SkeletonAgent agent;
    private Dictionary<int, float> lastHitTimes = new();

    void Update()
    {
        if (!canDealDamage || tip == null || basePoint == null) return;

        Vector3 origin = basePoint.position;
        Vector3 direction = (tip.position - origin).normalized;
        float distance = Vector3.Distance(origin, tip.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, targetLayer))
        {
            int targetID = hit.collider.GetInstanceID();
            float currentTime = Time.time;

            // Check cooldown for this specific target
            if (lastHitTimes.TryGetValue(targetID, out float lastHitTime))
            {
                if (currentTime - lastHitTime < targetCooldown) return;
            }

            IDamageable target = hit.transform.GetComponentInParent<IDamageable>();
            if (target != null && hit.transform != transform.root)
            {
                target.TakeDamage(weaponDamage);
                lastHitTimes[targetID] = currentTime;

                Debug.Log($"Dealt {weaponDamage} damage to {hit.transform.name}");

                if (agent != null)
                    agent.ReportSuccessfulHit();

                hasDealtDamage = true;
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
        Debug.Log("Started dealing damage.");
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
        Debug.Log("Stopped dealing damage.");
    }

    void OnDrawGizmos()
    {
        if (tip != null && basePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(basePoint.position, tip.position);
        }
    }
}
