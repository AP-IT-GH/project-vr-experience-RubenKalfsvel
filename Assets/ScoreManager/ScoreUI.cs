using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] public ScoreManager scoreManager;

    private int lastScore = -1;

    void Start()
    {

        // Initial score display
        UpdateScoreDisplay();
    }

    void Update()
    {
        // Update score display if it changed
        if (scoreManager != null)
        {
            int currentScore = scoreManager.GetScore();
            if (currentScore != lastScore)
            {
                UpdateScoreDisplay();
                lastScore = currentScore;
            }
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null && scoreManager != null)
        {
            scoreText.text = scorePrefix + scoreManager.GetScore().ToString();
        }
    }
}