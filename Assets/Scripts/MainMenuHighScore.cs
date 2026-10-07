using UnityEngine;
using TMPro;

/// <summary>
/// Displays the persistent high score loaded via PlayerPrefs on the Main Menu UI.
/// </summary>
public class MainMenuHighScore : MonoBehaviour
{
    private void Start()
    {
        TMP_Text highScoreText = GetComponent<TMP_Text>();

        if (highScoreText != null)
        {
            // Fetch the high score saved by the GameManager (defaults to 0 if none exists yet)
            int savedHighScore = PlayerPrefs.GetInt("PlayerHighScore", 0);

            // Update the UI text
            highScoreText.text = $"High Score: {savedHighScore}";
        }
    }
}
