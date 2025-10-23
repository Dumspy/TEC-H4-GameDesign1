using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    public bool timerStarted = false;
    public bool timerEnded = false;
    public float finalElapsedTime = 0f;
    public float startTime = 0f;
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string formattedTime = "00:00.00";
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Label>("TimerLabel").dataSource = this;
    }
    void Update()
    {
        if (timerStarted && !timerEnded)
        {
            float currentTime = Time.time - startTime;
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            int milliseconds = Mathf.FloorToInt(currentTime * 100f % 100f);
            formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }
    }
    public void StartTimer()
    {
        if (!timerStarted)
        {
            timerStarted = true;
            startTime = Time.time;
        }
    }
    public void OnGoalReached()
    {
        if (timerStarted && !timerEnded)
        {
            timerEnded = true;
            finalElapsedTime = Time.time - startTime;
        }
    }
}
