using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pauseMenuUI;

    [Header("Player References")]
    public Transform playerHead;

    [Header("Positioning Settings")]
    public float distanceInFront = 2f;
    public float heightOffset = -0.2f;

    [Header("XR Components to Disable")]
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedSnapTurnProvider turnProvider;
    public XRDirectInteractor leftDirectInteractor;
    public XRDirectInteractor rightDirectInteractor;
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    private bool isPaused = false;

    void Update()
    {
        // todo: replace with vr button to pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        PositionPauseScreen();

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // disable vr inputs
        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;

        if (leftDirectInteractor != null) leftDirectInteractor.enabled = false;
        if (rightDirectInteractor != null) rightDirectInteractor.enabled = false;
        if (leftRayInteractor != null) leftRayInteractor.enabled = false;
        if (rightRayInteractor != null) rightRayInteractor.enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // enable vr inputs
        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;

        if (leftDirectInteractor != null) leftDirectInteractor.enabled = true;
        if (rightDirectInteractor != null) rightDirectInteractor.enabled = true;
        if (leftRayInteractor != null) leftRayInteractor.enabled = true;
        if (rightRayInteractor != null) rightRayInteractor.enabled = true;
    }

    private void PositionPauseScreen()
    {
        Vector3 forward = playerHead.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 newPos = playerHead.position + forward * distanceInFront;
        newPos.y += heightOffset;

        pauseMenuUI.transform.position = newPos;
        pauseMenuUI.transform.rotation = Quaternion.LookRotation(forward);
    }
}
