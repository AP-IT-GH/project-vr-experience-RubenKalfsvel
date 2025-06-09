using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] private int score = 0;
    [SerializeField] private int pointsPerKill = 10;

    public void IncreaseScore(int points = -1)
    {
        int pointsToAdd = points == -1 ? pointsPerKill : points;
        score += pointsToAdd;
        Debug.Log($"Score increased by {pointsToAdd}! Current score: {score}");
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        Debug.Log("Score reset to 0");
    }

    public void SetPointsPerKill(int points)
    {
        pointsPerKill = points;
    }

}
