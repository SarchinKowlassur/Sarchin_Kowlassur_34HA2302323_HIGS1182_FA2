using UnityEngine;

/// <summary>
/// Scales difficulty over time, reaching max intensity over a 60-second period.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    [Header("Scaling Settings")]
    // Ramps multiplier from 1x up to 3x across 60 seconds
    [SerializeField] private float difficultyIncreaseRate = 0.033f; // (0.033 * 60 = 2.0 increase, totaling 3.0x at 60s)

    public float CurrentDifficultyMultiplier { get; private set; } = 1f;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        CurrentDifficultyMultiplier += difficultyIncreaseRate * Time.deltaTime;
        CurrentDifficultyMultiplier = Mathf.Min(CurrentDifficultyMultiplier, 3f); // Hard cap at max difficulty
    }
}