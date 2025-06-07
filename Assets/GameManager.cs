using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject victoryScreen;
    public Transform playerHead;

    public void ShowDeathScreen()
    {
        // todo: call this method from another script when player dies
        deathScreen.SetActive(true);
        PositionScreen(deathScreen);
        Time.timeScale = 0f;
    }

    public void ShowVictoryScreen()
    {
        // todo: call this method from another script when player kills all enemies
        victoryScreen.SetActive(true);
        PositionScreen(victoryScreen);
        Time.timeScale = 0f;
    }

    void PositionScreen(GameObject screen)
    {
        screen.transform.position = playerHead.position + playerHead.forward * 2f;
        screen.transform.rotation = Quaternion.LookRotation(playerHead.forward);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }
}
