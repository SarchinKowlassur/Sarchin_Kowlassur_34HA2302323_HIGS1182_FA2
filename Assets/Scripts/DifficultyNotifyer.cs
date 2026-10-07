using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays on-screen feedback to the player at 20-second intervals up to 60 seconds.
/// </summary>
public class DifficultyNotifier : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Text alertText;

    [Header("Strict 20-Second Milestones")]
    [SerializeField] private float milestone1 = 20f; // Wave 2 at 20s
    [SerializeField] private float milestone2 = 40f; // Wave 3 at 40s
    [SerializeField] private float milestone3 = 60f; // Max Difficulty at 60s

    private bool triggered1 = false;
    private bool triggered2 = false;
    private bool triggered3 = false;
    private float survivalTimer = 0f;

    private void Start()
    {
        if (alertText != null)
        {
            alertText.text = ""; // Clear text on start
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        survivalTimer += Time.deltaTime;

        // Trigger notifications at 20, 40, and 60 second intervals
        if (!triggered1 && survivalTimer >= milestone1)
        {
            triggered1 = true;
            ShowAlert("Wave 2: Enemy Density Increasing!");
        }
        else if (!triggered2 && survivalTimer >= milestone2)
        {
            triggered2 = true;
            ShowAlert("Wave 3: Enemy Aggression Increased!");
        }
        else if (!triggered3 && survivalTimer >= milestone3)
        {
            triggered3 = true;
            ShowAlert("Warning: Max Difficulty Reached!");
        }
    }

    public void ShowAlert(string message)
    {
        if (alertText != null)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayAlertRoutine(message));
        }
    }

    private IEnumerator DisplayAlertRoutine(string message)
    {
        alertText.text = message;
        alertText.gameObject.SetActive(true);

        // Display on screen for 3 seconds
        yield return new WaitForSeconds(3f);

        alertText.text = "";
    }
}