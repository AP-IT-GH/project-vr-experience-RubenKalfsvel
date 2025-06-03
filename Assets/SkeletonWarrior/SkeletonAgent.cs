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
    public float attackCooldown = 2f;

    [Header("Rewards")]
    public float hitOpponentReward = 0.5f;
    public float tookDamagePenalty = -0.2f;
    public float deathPenalty = -1.0f;
    public float killReward = 1.0f;
    public float missedAttackPenalty = -0.05f;

    [Header("References")]
    private Animator animator;
    public GameObject weapon;
    public SkeletonAgent opponentAgent;
    public Transform opponentTransform;

    [Header("Distance Penalty")]
    public float distancePenalty = -0.02f; // 10x stronger
    public float tooFarDistance = 5f;

    private float currentHealth;
    private bool attackReady;

    private bool attackLanded;

    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        attackReady = true;
        currentHealth = maxHealth;
    }

    public override void OnEpisodeBegin()
    {
        ResetAgent();

        if (opponentAgent != null)
        {
            opponentAgent.ResetAgent();
        }
    }

    public void ResetAgent()
    {
        currentHealth = maxHealth;
        transform.localPosition = GetRandomSpawnPosition();
        transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        animator.SetFloat("speed", 0);
        animator.ResetTrigger("damage");
        animator.ResetTrigger("attack");
        animator.SetInteger("deathType", 0);

        attackReady = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(currentHealth / maxHealth);
        sensor.AddObservation(attackReady ? 1f : 0f);

        if (opponentTransform != null)
        {
            Vector3 toOpponent = opponentTransform.position - transform.position;
            sensor.AddObservation(toOpponent.normalized);
            sensor.AddObservation(toOpponent.magnitude / 10f); // normalize distance

            float angle = Vector3.Angle(transform.forward, toOpponent);
            sensor.AddObservation(angle / 180f); // normalized angle

            PlayerDummy opponentHealth = opponentTransform.GetComponent<PlayerDummy>();
            sensor.AddObservation(opponentHealth != null ? opponentHealth.currentHealth / 3f : 0f);
        }
        else
        {
            // Pad observations if opponent is missing
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveInput = actions.ContinuousActions[0];
        float rotateInput = actions.ContinuousActions[1];
        int combatAction = actions.DiscreteActions[0];

        // Movement & Rotation
        transform.Translate(transform.forward * moveInput * moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(transform.up, rotateInput * rotationSpeed * Time.deltaTime);
        animator.SetFloat("speed", moveInput);

        // Combat Actions
        if (combatAction == 1 && attackReady)
        {
            AttemptAttack();
            StartCoroutine(AttackCooldownCoroutine(attackCooldown));
        }

        // Penalty for being too far from opponent
        if (opponentTransform != null)
        {
            float distance = Vector3.Distance(transform.position, opponentTransform.position);
            if (distance > tooFarDistance)
            {
                AddReward(distancePenalty);
            }
            float combatDistance = 2.5f;
            if (distance < combatDistance)
            {
                AddReward(0.01f); // Encourage staying close
            }

        }


        // Tiny time penalty to encourage efficiency
        AddReward(-0.001f / MaxStep);
    }

    IEnumerator AttackCooldownCoroutine(float cooldown)
    {
        attackReady = false;
        yield return new WaitForSeconds(cooldown);
        attackReady = true;
    }

    void AttemptAttack()
    {
        attackLanded = false; // reset before swing
        animator.SetTrigger("attack");

        // Schedule check for miss after cooldown
        StartCoroutine(CheckAttackMissedAfterDelay(attackCooldown));
    }

    IEnumerator CheckAttackMissedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!attackLanded)
        {
            AddReward(missedAttackPenalty);
            Debug.Log("Missed attack! Applied penalty.");
        }
    }


    public void StartDealDamage()
    {
        weapon.GetComponentInChildren<EnemyDamageDealer>()?.StartDealDamage();
    }

    public void EndDealDamage()
    {
        weapon.GetComponentInChildren<EnemyDamageDealer>()?.EndDealDamage();
    }

    // Called by EnemyDamageDealer when the agent lands a hit
    public void ReportSuccessfulHit()
    {
        attackLanded = true;
        AddReward(hitOpponentReward);

        if (opponentAgent != null)
        {
            opponentAgent.TakeDamage(1f);
            if (opponentAgent.currentHealth <= 0)
            {
                AddReward(killReward);
                EndEpisode();
                opponentAgent.AddReward(deathPenalty);
                opponentAgent.EndEpisode();
            }
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("damage");
        AddReward(tookDamagePenalty);

        Debug.Log($"{name} took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        int deathType = Random.Range(1, 4);
        animator.SetInteger("deathType", deathType);
        AddReward(deathPenalty);
        EndEpisode();
    }

    Vector3 GetRandomSpawnPosition()
    {
        float range = 3f;
        return new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
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
