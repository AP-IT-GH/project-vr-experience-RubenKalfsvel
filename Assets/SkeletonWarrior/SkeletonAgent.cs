using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

public class SkeletonAgent : Agent
{
    [Header("Agent Stats")]
    public float maxHealth = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 100f;
    public float attackCooldown = 3f;


    [Header("Rewards")]
    public float hitPlayerReward = 0.5f;
    public float tookDamagePenalty = -0.2f;
    public float deathPenalty = -1.0f;
    public float missedAttackPenalty = -0.05f; 

    [Header("References")]
    private Animator animator;
    private Transform playerTransform;
    public GameObject weapon;





    private float currentHealth;
    private bool AttackReady;


    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerTransform = playerObj.transform;
        AttackReady = true; 


    }

    public override void OnEpisodeBegin()
    {
        
        currentHealth = maxHealth;
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        animator.SetFloat("speed", 0);
        animator.ResetTrigger("damage");
        animator.ResetTrigger("attack");
        animator.SetInteger("deathType", 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations:
        // 1. Agent's normalized health (1)
        // 2. Agent attack ready (1)

        // Total = 2

        sensor.AddObservation(currentHealth / maxHealth); // Observe own health
        sensor.AddObservation(AttackReady ? 1f : 0f); // Observe if attack is ready


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
        animator.SetFloat("speed", moveInput);

        // Attack Action
        if (attackAction == 1)
        {

            if (AttackReady == false) {

                return;
            
            }

            AttemptAttack();

            StartCoroutine(AttackCooldownCoroutine(attackCooldown));

        }

        // Small time penalty
        AddReward(-0.001f / MaxStep);

    }

    IEnumerator AttackCooldownCoroutine(float cooldown)
    {
        AttackReady = false;
        yield return new WaitForSeconds(cooldown);
        AttackReady = true;
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
        int deathType = Random.Range(1, 4); // 0, 1, or 2
        animator.SetInteger("DeathType" , deathType);
        AddReward(deathPenalty);
        EndEpisode();
    }

    public void StartDealDamage()
    {
        weapon.GetComponentInChildren<EnemyDamageDealer>().StartDealDamage();
    }
    public void EndDealDamage()
    {
        weapon.GetComponentInChildren<EnemyDamageDealer>().EndDealDamage();
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