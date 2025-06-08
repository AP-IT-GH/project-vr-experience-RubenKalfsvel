using UnityEngine;

public class SwordSwingDamageTrigger : MonoBehaviour
{
    public EnemyDamageDealer damageDealer;
    public float swingSpeedThreshold = 1.5f;
    public float minDealTime = 0.1f; // cooldown

    private Vector3 lastPosition;
    private float lastTriggerTime = -999f;
    private bool isDealing = false;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        float speed = velocity.magnitude;

        if (!isDealing && speed > swingSpeedThreshold)
        {
            damageDealer.StartDealDamage();
            isDealing = true;
            lastTriggerTime = Time.time;
        }

        if (isDealing && Time.time > lastTriggerTime + minDealTime)
        {
            damageDealer.EndDealDamage();
            isDealing = false;
        }

        lastPosition = transform.position;
    }
}
