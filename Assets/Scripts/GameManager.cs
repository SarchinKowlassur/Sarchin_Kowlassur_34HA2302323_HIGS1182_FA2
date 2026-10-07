using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Singleton GameManager controlling score, high score persistence via PlayerPrefs, win/loss states, and UI screens.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Win Conditions")]
    [SerializeField] private int scrapToWin = 25; // Set required target scrap count

    [Header("UI Text References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText; // Reference for high score UI text

    [Header("UI Panel References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public bool IsGameOver { get; private set; }

    private const string HighScoreKey = "PlayerHighScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CurrentScore = 0;
        IsGameOver = false;

        // Load Persistent High Score from PlayerPrefs (defaults to 0 if none saved)
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        UpdateScoreUI();
    }

    // Call to add score when scrap is collected. Checks win condition and updates persistent high score.
    public void AddScore(int amount)
    {
        if (IsGameOver) return;

        CurrentScore += amount;

        // Check if current score beats the high score
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save(); // Save data persistently
            Debug.Log("New High Score Saved: " + HighScore);
        }

        UpdateScoreUI();

        // Check if player reached the winning threshold
        if (CurrentScore >= scrapToWin)
        {
            GameWin();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Scrap: {CurrentScore} / {scrapToWin}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {HighScore}";
        }
    }

   
    //Triggers the Win state and opens the Win Screen.
    public void GameWin()
    {
        IsGameOver = true;
        Debug.Log("Player won the game by collecting all required scrap!");

        if (gameWinPanel != null)
        {
            gameWinPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

 
    // Triggers the Game Over loss state.
    public void GameOver(bool won = false)
    {
        if (won || IsGameOver) return;

        IsGameOver = true;
        Debug.Log("Game Over - Player destroyed!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // UI Button Methods
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}