using Unity.Properties;
using UnityEngine.UIElements;
using UnityEngine;

[RequireComponent(typeof(UIDocument))]
public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }
    public Collectable[] collectables;
    private int maxScore = 0;
    private int score = 0;
    [UxmlAttribute, CreateProperty, HideInInspector]
    public string formattedScore = "0";
    public int BananasCollected => score;
    public int MaxBananas => maxScore;
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
        root.Q<Label>("ScoreLabel").dataSource = this;
    }
    void Start()
    {
        if (collectables != null)
        {
            foreach (var collectable in collectables)
            {
                if (collectable != null)
                    collectable.OnCollected = OnCollectableCollected;
            }
            maxScore = collectables.Length;
        }
        formattedScore = $"{score}/{maxScore}";
    }
    private void OnCollectableCollected()
    {
        score++;
        formattedScore = $"{score}/{maxScore}";
    }
}
