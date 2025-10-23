using Unity.Properties;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardController : MonoBehaviour {
    private UIDocument leaderboardUIDocument;
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string formattedLeaderboard = "";
    public static LeaderboardController Instance { get; private set; }
    private string LeaderboardFile => Path.Combine(Application.persistentDataPath, "leaderboard.json");
    public LeaderboardData leaderboard = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadLeaderboard();
        UpdateFormattedLeaderboard();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            leaderboardUIDocument = FindFirstObjectByType<UIDocument>();
            if (leaderboardUIDocument != null)
            {
                VisualElement root = leaderboardUIDocument.rootVisualElement;
                var label = root.Q<Label>("LeaderboardLabel");
                if (label != null)
                    label.dataSource = this;
            }
        }
    }
    public void AddEntry(string playerName, float elapsedTime, int bananasCollected, int maxBananas, int timePenaltySeconds)
    {
        int missingBananas = maxBananas - bananasCollected;
        float finalTime = elapsedTime + missingBananas * timePenaltySeconds;
        leaderboard.entries.Add(new LeaderboardEntry {
            playerName = playerName,
            time = finalTime,
            score = bananasCollected
        });
        leaderboard.entries.Sort((a, b) =>
        {
            int timeCompare = a.time.CompareTo(b.time);
            if (timeCompare != 0) return timeCompare;
            return b.score.CompareTo(a.score);
        });
        SaveLeaderboard();
        UpdateFormattedLeaderboard();
    }
    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboard, true);
        File.WriteAllText(LeaderboardFile, json);
    }
    private void LoadLeaderboard()
    {
        if (File.Exists(LeaderboardFile))
        {
            string json = File.ReadAllText(LeaderboardFile);
            leaderboard = JsonUtility.FromJson<LeaderboardData>(json);
            UpdateFormattedLeaderboard();
        }
        else
        {
            leaderboard = new LeaderboardData();
            UpdateFormattedLeaderboard();
        }
    }
    private void UpdateFormattedLeaderboard()
    {
        if (leaderboard.entries.Count == 0)
        {
            formattedLeaderboard = "No entries yet.";
            return;
        }
        System.Text.StringBuilder sb = new();
        int rank = 1;
        foreach (var entry in leaderboard.entries)
        {
            int minutes = Mathf.FloorToInt(entry.time / 60f);
            int seconds = Mathf.FloorToInt(entry.time % 60f);
            int milliseconds = Mathf.FloorToInt(entry.time * 100f % 100f);
            string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            sb.AppendLine($"{rank}. {entry.playerName} - {formattedTime} - {entry.score}");
            rank++;
        }
        formattedLeaderboard = sb.ToString();
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public int score;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new();
}
