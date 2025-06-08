using UnityEngine;

public class PlayerDamageDealer : MonoBehaviour
{
    public float damage = 1f;
    public float attackRange = 1.5f;
    public LayerMask targetLayer;
    public Transform raycastOrigin;
    public float raycastRadius = 0.3f;

    private bool canDealDamage = false;

    private void Start()
    {
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
        }
    }

    void Update()
    {
        if (!canDealDamage) return;

        if (Physics.SphereCast(raycastOrigin.position, raycastRadius, raycastOrigin.forward, out RaycastHit hit, attackRange, targetLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && hit.transform != transform.root)
            {
                target.TakeDamage(damage);
                Debug.Log($"Dealt {damage} damage to {hit.transform.name}");
                canDealDamage = false;
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