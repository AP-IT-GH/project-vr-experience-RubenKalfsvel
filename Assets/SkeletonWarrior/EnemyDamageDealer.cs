using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    bool canDealDamage;
    bool hasDealtDamage;

    [SerializeField] float weaponLength;
    [SerializeField] float weaponDamage;

    private SkeletonAgent agent; 

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = false;
        agent = GetComponentInParent<SkeletonAgent>();
 
    }

    void Update()
    {

        
        if (canDealDamage && !hasDealtDamage)
        {
            RaycastHit hit;

            int layerMask = 1 << 8; //  Layer 8 for Player
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
            {

                print("Hit: " + hit.transform.name);
                
                if (hit.transform.TryGetComponent(out PlayerDummy health)) 
                {
                    
                    health.TakeDamage(weaponDamage);
                    

                    
                    /*if (agent != null)
                    {
                        agent.ReportSuccessfulHit(); 
                    }*/

                    // Prevent dealing damage multiple times per swing
                    hasDealtDamage = true;
                }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}