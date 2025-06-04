using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    private bool canDealDamage;
    private bool hasDealtDamage;

    [SerializeField] private Transform tip; // Assign in Inspector
    [SerializeField] private float weaponDamage = 1f;
    [SerializeField] private LayerMask targetLayer; // Set to "Agent" or custom layer

    private SkeletonAgent agent;

    void Start()
    {
        agent = GetComponentInParent<SkeletonAgent>();
    }

    void Update()
    {
        if (!canDealDamage || hasDealtDamage || tip == null) return;

        Vector3 origin = transform.position;         // Pommel position
        Vector3 direction = (tip.position - origin).normalized;
        float distance = Vector3.Distance(origin, tip.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, targetLayer))
        {
            if (hit.transform.TryGetComponent(out SkeletonAgent targetAgent) && targetAgent != agent)
            {
                targetAgent.TakeDamage(weaponDamage);
                agent?.ReportSuccessfulHit();
                hasDealtDamage = true;
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    void OnDrawGizmos()
    {
        if (tip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, tip.position);
        }
    }
}
