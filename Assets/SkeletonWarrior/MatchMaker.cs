using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public float matchDuration = 30f;
    private float timer;

    public SkeletonAgent agentA;
    public SkeletonAgent agentB;

    void OnEnable()
    {
        timer = matchDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (agentA != null && agentA.enabled)
            {
                agentA.AddReward(-0.2f); // Optional timeout penalty
                agentA.EndEpisode();
            }

            if (agentB != null && agentB.enabled)
            {
                agentB.AddReward(-0.2f);
                agentB.EndEpisode();
            }

            timer = matchDuration;
        }
    }
}
