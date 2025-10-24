using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldEscToMenu : MonoBehaviour
{
    [Header("Hold ESC to return to menu")]
    [Tooltip("Seconds the ESC key must be held to return to main menu.")]
    public float holdDuration = 2f;
    [Tooltip("Name of the main menu scene.")]
    public string mainMenuSceneName = "MainMenu";

    private float escHeldTime = 0f;
    private bool escWasHeld = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            escHeldTime += Time.unscaledDeltaTime;
            escWasHeld = true;
            if (escHeldTime >= holdDuration)
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
        else
        {
            if (escWasHeld)
            {
                escHeldTime = 0f;
                escWasHeld = false;
            }
        }
    }
}
