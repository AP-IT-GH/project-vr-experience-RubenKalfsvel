using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("ColosseumScene");
    }

    public void QuitGame()
    {
        // this only works in an actual built application, not in unity editor
        Application.Quit();
    }
}
