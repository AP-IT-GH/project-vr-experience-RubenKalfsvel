using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 1f;
    public float attackRange = 1.5f;
    public LayerMask targetLayer;
    public Transform raycastOrigin;
    public float raycastRadius = 0.3f;

    private bool canDealDamage = false;
    private SkeletonAgent agentOwner;

    private void Start()
    {
        agentOwner = GetComponentInParent<SkeletonAgent>();
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
        }
    }

    void Update()
    {
        if (!canDealDamage) return;

        // Use SphereCast for more forgiving hit detection
        if (Physics.SphereCast(raycastOrigin.position, raycastRadius, raycastOrigin.forward, out RaycastHit hit, attackRange, targetLayer))
        {
            var opponent = hit.collider.GetComponent<SkeletonAgent>();
            if (opponent != null && opponent != agentOwner)
            {
                opponent.TakeDamage(damage);
                agentOwner.ReportSuccessfulHit();
                canDealDamage = false; // Prevent multiple hits in one attack
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }
}
