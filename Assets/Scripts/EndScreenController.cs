using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(TimeController))]
[RequireComponent(typeof(ScoreController))]
public class EndScreenController : MonoBehaviour {
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string FormattedBananasCollected { get; set; }
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string FormattedMissingBananas { get; set; }
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string FormattedTimePenalty { get; set; }
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string FormattedTimeSpent { get; set; }
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string FormattedFinalTime { get; set; }
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string Username { get; set; } = "";

    public int BananasCollected { get; private set; }
    public int MissingBananas { get; private set; }

    [Header("Settings")]
    [SerializeField]
    private int timePenaltySeconds = 15;
    [Header("References")]
    public PlayerController playerController;
    public string mainMenuScene = "MainMenu";

    private void OnEnable() {
        if (TimeController.Instance.timerStarted && !TimeController.Instance.timerEnded) return;

        var root = GetComponent<UIDocument>().rootVisualElement;
        var gameUI = root.Q<VisualElement>("GameUI");
        var endUI = root.Q<VisualElement>("EndUI");
        if (gameUI != null) gameUI.style.display = DisplayStyle.None;
        if (endUI != null) endUI.style.display = DisplayStyle.Flex;

        if (playerController != null)
            playerController.DisableMovement();

        // UI label bindings
        root.Q<Label>("EndTimerLabel")?.SetBinding(this, nameof(FormattedTimeSpent));
        root.Q<Label>("BananasCollectedLabel")?.SetBinding(this, nameof(FormattedBananasCollected));
        root.Q<Label>("MissingBananasLabel")?.SetBinding(this, nameof(FormattedMissingBananas));
        root.Q<Label>("TimePenaltyLabel")?.SetBinding(this, nameof(FormattedTimePenalty));
        root.Q<Label>("FinalTimeLabel")?.SetBinding(this, nameof(FormattedFinalTime));

        var inputField = root.Q<TextField>("UsernameInput");
        var submitBtn = root.Q<Button>("SubmitButton");
        var returnBtn = root.Q<Button>("ReturnButton");

        // Calculate bananas collected and missing
        BananasCollected = ScoreController.Instance.BananasCollected;
        MissingBananas = ScoreController.Instance.MaxBananas - BananasCollected;

        // Set formatted strings for UI binding
        FormattedBananasCollected = $"Bananas Collected: {BananasCollected}";
        FormattedMissingBananas = $"Missing Bananas: {MissingBananas}";
        FormattedTimePenalty = $"Time Penalty: {MissingBananas} x {timePenaltySeconds} seconds";

        float elapsed = TimeController.Instance.finalElapsedTime;
        int spentMinutes = Mathf.FloorToInt(elapsed / 60f);
        int spentSeconds = Mathf.FloorToInt(elapsed % 60f);
        int spentMilliseconds = Mathf.FloorToInt(elapsed * 100f % 100f);
        FormattedTimeSpent = string.Format("Time spent: {0:00}:{1:00}.{2:00}", spentMinutes, spentSeconds, spentMilliseconds);
        float finalTime = elapsed + MissingBananas * timePenaltySeconds;
        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);
        int milliseconds = Mathf.FloorToInt(finalTime * 100f % 100f);
        FormattedFinalTime = string.Format("Final time: {0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        // Submit to leaderboard
        if (submitBtn != null)
        {
            submitBtn.clicked += () =>
            {
                string username = (inputField != null && !string.IsNullOrWhiteSpace(inputField.text)) ? inputField.text : "Player";
                LeaderboardController.Instance?.AddEntry(username, TimeController.Instance.finalElapsedTime, BananasCollected, ScoreController.Instance.MaxBananas, timePenaltySeconds);
                submitBtn.SetEnabled(false);
            };
        }

        // Return to main menu
        if (returnBtn != null)
        {
            returnBtn.clicked += () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
            };
        }

        // Data binding for username input
        if (inputField != null)
        {
            inputField.dataSource = this;
            inputField.bindingPath = nameof(Username);
        }
    }
}

// Extension method for label binding
public static class VisualElementExtensions
{
    public static void SetBinding(this Label label, object dataSource, string bindingPath)
    {
        if (label != null)
        {
            label.dataSource = dataSource;
            label.bindingPath = bindingPath;
        }
    }
}
