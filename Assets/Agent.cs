using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentScript : Agent
{
    public GameObject Target;

    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(6.5f, 13.15f, 17.2f);
        this.transform.localRotation = Quaternion.identity;

        Target.transform.localPosition = new Vector3(6.76f, 13.15f, 2.7f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition.x);
        sensor.AddObservation(this.transform.localPosition.z);

    }

    public float speedMultiplier = 0.1f;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        transform.Translate(controlSignal * speedMultiplier);

        // Kleine negatieve beloning voor tijd
        AddReward(-0.001f);

        float distanceToTarget = Vector3.Distance(
            new Vector3(this.transform.localPosition.x, 0, this.transform.localPosition.z),
            new Vector3(Target.transform.localPosition.x, 0, Target.transform.localPosition.z)
        );

        // Target bereikt
        if (distanceToTarget < 0.5f)
        {
            AddReward(1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    // Optioneel: implementeer hit detection
    private void OnTriggerEnter(Collider other)
    {
    }
}