using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("References")]
    public EndScreenController endScreenController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TimeController.Instance != null)
        {
            TimeController.Instance.OnGoalReached();
            if (endScreenController != null)
                endScreenController.enabled = true;
        }
    }
}
