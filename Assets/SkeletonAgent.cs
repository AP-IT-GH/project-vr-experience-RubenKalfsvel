using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SkeletonAgent : Agent
{
    [Header("Agent Stats")]
    public float maxHealth = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 100f;
    public float attackRange = 1.5f;
    public float attackCooldown = 3f;


    [Header("Rewards")]
    public float hitPlayerReward = 0.5f;
    public float tookDamagePenalty = -0.2f;
    public float deathPenalty = -1.0f;
    public float missedAttackPenalty = -0.05f; 

    [Header("References")]
    private Animator animator;
    private Transform playerTransform;



    private float currentHealth;


    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindWithTag("Player");

    }

    public override void OnEpisodeBegin()
    {
        
        currentHealth = maxHealth;
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        animator.SetFloat("speed", 0);
        animator.ResetTrigger("damage");
        animator.ResetTrigger("attack");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations:
        // 1. Agent's normalized health (1 float)
        // 2. Normalized direction to player (3 floats)
        // 3. Distance to player (1 float)
        // 4. Can attack player (boolean -> float, 1 float)
        // Total = 6 floats

        sensor.AddObservation(currentHealth / maxHealth); // Observe own health

        if (playerTransform == null)
        {
            sensor.AddObservation(Vector3.zero); // Relative position (normalized)
            sensor.AddObservation(0f);           // Distance
            sensor.AddObservation(false);        // Can Attack
            return;
        }

        Vector3 toPlayer = playerTransform.localPosition - transform.localPosition;
        sensor.AddObservation(toPlayer.normalized); // Use normalized direction
        float distanceToPlayer = toPlayer.magnitude; // More efficient than Vector3.Distance
        sensor.AddObservation(distanceToPlayer);
        bool canAttack = (distanceToPlayer <= attackRange);
        sensor.AddObservation(canAttack);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
      
        // Continuous Action 0: Forward/Backward movement (-1 to +1)
        // Continuous Action 1: Rotation Left/Right (-1 to +1)
        // Discrete Action Branch 0, Action 1: Attack

        float moveInput = actions.ContinuousActions[0];
        float rotateInput = actions.ContinuousActions[1];
        int attackAction = actions.DiscreteActions[0];

        // Movement & Rotation
        transform.Translate(transform.forward * moveInput * moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(transform.up, rotateInput * rotationSpeed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(moveInput));

        // Attack Action
        if (attackAction == 1)
        {
            AttemptAttack();
        }

        // Small time penalty
        AddReward(-0.001f / MaxStep);
    }

    void AttemptAttack()
    {
        animator.SetTrigger("attack");
    }

    // --- This method is called by EnemyDamageDealer ---
    public void ReportSuccessfulHit()
    {

        Debug.Log("Agent reported successful hit! Applying reward.");
        AddReward(hitPlayerReward);

       
        PlayerDummy playerHealth = playerTransform.GetComponent<PlayerDummy>();
        if (playerHealth != null && playerHealth.currentHealth <= 0) {
             AddReward(1.0f); // Big bonus for kill
             EndEpisode();
         }
    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        animator.SetTrigger("damage");
        AddReward(tookDamagePenalty);
        Debug.Log($"Agent took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        AddReward(deathPenalty);
        EndEpisode();
    }

    public void StartDealDamage()
    {
        GetComponent<EnemyDamageDealer>().StartDealDamage();
    }
    public void EndDealDamage()
    {
        GetComponent<EnemyDamageDealer>().EndDealDamage();
    }
     

    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical"); 
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); 
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;

    }

}