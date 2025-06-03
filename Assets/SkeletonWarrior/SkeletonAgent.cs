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
    public float attackCooldown = 3f;

    [Header("References")]
    private Animator animator;
    private Transform playerTransform;
    public GameObject weapon;

    private float previousDistance;

    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        playerTransform = playerObj.transform;
    }

    public override void OnEpisodeBegin()
    {
        transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        animator.SetFloat("speed", 0);
        animator.ResetTrigger("damage");
        animator.ResetTrigger("attack");
        animator.SetInteger("deathType", 0);
        previousDistance = Vector3.Distance(transform.position, playerTransform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        sensor.AddObservation(directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        sensor.AddObservation(distanceToPlayer);
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveInput = actions.ContinuousActions[0];
        float rotateInput = actions.ContinuousActions[1];

        transform.Translate(transform.forward * moveInput * moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(transform.up, rotateInput * rotationSpeed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(moveInput));

        float currentDistance = Vector3.Distance(transform.position, playerTransform.position);
        if (StepCount > 0)
        {
            float reward = (previousDistance - currentDistance) * 0.1f;
            AddReward(reward);
        }
        previousDistance = currentDistance;

        AddReward(-0.001f);

        if (currentDistance > 10f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        else if (currentDistance < 1f)
        {
            AddReward(5.0f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AddReward(5.0f);
            EndEpisode();
        }
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