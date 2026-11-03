using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ===== 全局游戏状态 =====
    public static bool IsGameplayActive { get; private set; } = true;

    // ===== 结算界面 =====
    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject gameClearPanel;

    // ===== 暂停界面 =====
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject startPanel;

    // ===== 分数界面 =====
    [SerializeField]
    private TMP_Text scoreText;

    // ===== 运行时状态 =====
    private int score;
    private bool isGameEnded;
    private bool isPaused;
    private bool isGameStarted;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 没有设置开始界面时保持原来的直接开始行为。
        isGameStarted = startPanel == null;
        IsGameplayActive = isGameStarted;
        Time.timeScale = isGameStarted ? 1f : 0f;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        score = 0;
        UpdateScoreText();
    }

    private void Update()
    {
        if (!isGameEnded &&
            isGameStarted &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isGameEnded &&
            Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // ===== 分数管理 =====
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text =
            "SCORE " + score.ToString("D6");
    }

    // ===== 游戏结束 =====
    public void GameOver()
    {
        EndGame(gameOverPanel);
    }

    public void GameClear()
    {
        EndGame(gameClearPanel);
    }

    private void EndGame(GameObject panel)
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        isPaused = false;
        IsGameplayActive = false;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        panel.SetActive(true);
    }

    // ===== 暂停与继续 =====
    public void TogglePause()
    {
        if (isGameEnded) return;

        isPaused = !isPaused;
        IsGameplayActive = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        TogglePause();
    }

    // ===== 开始游戏 =====
    public void StartGame()
    {
        if (isGameEnded) return;

        isGameStarted = true;
        isPaused = false;
        IsGameplayActive = true;
        Time.timeScale = 1f;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    // ===== 场景重开 =====
    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ===== 退出游戏 =====
    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
