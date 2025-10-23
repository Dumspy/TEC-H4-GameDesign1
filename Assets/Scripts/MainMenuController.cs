using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = "GameScene";

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startBtn = root.Q<Button>("StartButton");
        if (startBtn != null)
            startBtn.clicked += OnStartButton;

        Button exitBtn = root.Q<Button>("ExitButton");
        if (exitBtn != null)
            exitBtn.clicked += OnExitButton;
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnExitButton()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
