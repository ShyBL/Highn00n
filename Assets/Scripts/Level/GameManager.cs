using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main game manager that coordinates between LevelManager, PlayerStats, and UI
/// Handles game state, level progression, and save/load functionality
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PlayerStats playerStats;

    [Header("UI References")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Text levelDisplayText;
    [SerializeField] private Text characterDisplayText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button returnToMenuButton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseMenuButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool autosaveOnLevelComplete = true;

    // Game State
    private bool isPaused = false;
    private bool isGameActive = false;

    // Events
    public System.Action OnGameStarted;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnGameEnded;

    // Properties
    public bool IsPaused => isPaused;
    public bool IsGameActive => isGameActive;
    public LevelManager LevelManager => levelManager;

    private void Awake()
    {
        ValidateReferences();
        SetupEventListeners();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }

    private void ValidateReferences()
    {
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (levelManager == null)
            Debug.LogError("LevelManager not found! Please assign it in the inspector.");

        if (playerStats == null)
            Debug.LogError("PlayerStats not found! Please assign it in the inspector.");
    }

    private void SetupEventListeners()
    {
        // Level Manager events
        if (levelManager != null)
        {
            levelManager.OnLevelStarted += OnLevelStarted;
            levelManager.OnLevelCompleted += OnLevelCompleted;
            levelManager.OnLevelProgressChanged += OnLevelProgressChanged;
        }

        // UI Button events
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (restartLevelButton != null)
            restartLevelButton.onClick.AddListener(RestartCurrentLevel);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (pauseRestartButton != null)
            pauseRestartButton.onClick.AddListener(() => { ResumeGame(); RestartCurrentLevel(); });

        if (pauseMenuButton != null)
            pauseMenuButton.onClick.AddListener(() => { ResumeGame(); ReturnToMainMenu(); });
    }

    private void RemoveEventListeners()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelStarted -= OnLevelStarted;
            levelManager.OnLevelCompleted -= OnLevelCompleted;
            levelManager.OnLevelProgressChanged -= OnLevelProgressChanged;
        }

        // Remove UI button listeners if needed
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(PauseGame);

        if (restartLevelButton != null)
            restartLevelButton.onClick.RemoveListener(RestartCurrentLevel);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.RemoveListener(ReturnToMainMenu);

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
    }

    private void InitializeGame()
    {
        // Ensure game is not paused
        Time.timeScale = 1f;
        isPaused = false;

        // Hide pause menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Show game UI
        if (gameUI != null)
            gameUI.SetActive(true);

        // Update UI with session data
        UpdateGameUI();

        // Mark game as active
        isGameActive = true;
        OnGameStarted?.Invoke();

        Debug.Log("Game initialized successfully");
    }

    private void UpdateGameUI()
    {
        if (GameSessionData.Instance.IsSessionActive)
        {
            // Update level display
            if (levelDisplayText != null)
                levelDisplayText.text = $"Level: {GameSessionData.Instance.CurrentLevel}";

            // Update character display
            if (characterDisplayText != null)
                characterDisplayText.text = $"Character: {GameSessionData.Instance.CurrentCharacter}";
        }
        else
        {
            // Fallback display
            if (levelDisplayText != null)
                levelDisplayText.text = "Level: 1";

            if (characterDisplayText != null)
                characterDisplayText.text = "Character: Lloyd";
        }
    }

    private void OnLevelStarted(int level)
    {
        Debug.Log($"Game Manager: Level {level} started");

        // Update UI
        if (levelDisplayText != null)
            levelDisplayText.text = $"Level: {level}";

        // Any additional level start logic here
    }

    private void OnLevelCompleted(int level)
    {
        Debug.Log($"Game Manager: Level {level} completed");

        // Save progress if autosave is enabled
        if (autosaveOnLevelComplete)
        {
            SaveGameProgress();
        }

        // Show level complete effects, rewards, etc.
        ShowLevelCompleteEffects(level);
    }

    private void OnLevelProgressChanged(int currentLevel, int maxLevel)
    {
        Debug.Log($"Game Manager: Level progress {currentLevel}/{maxLevel}");

        // Update UI
        if (levelDisplayText != null)
            levelDisplayText.text = $"Level: {currentLevel}";

        // Update any progress bars or indicators here
    }

    private void ShowLevelCompleteEffects(int level)
    {
        // Add visual/audio feedback for level completion
        // This could include:
        // - Playing completion sound
        // - Showing completion animation
        // - Displaying rewards earned
        // - Unlocking new characters (if applicable)

        Debug.Log($"Level {level} completed! Add completion effects here.");
    }

    public void PauseGame()
    {
        if (!isGameActive) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (gameUI != null)
            gameUI.SetActive(false);

        OnGamePaused?.Invoke();
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (gameUI != null)
            gameUI.SetActive(true);

        OnGameResumed?.Invoke();
        Debug.Log("Game resumed");
    }

    public void RestartCurrentLevel()
    {
        if (levelManager != null)
        {
            levelManager.RestartLevel();
            Debug.Log("Level restarted");
        }
    }

    public void ReturnToMainMenu()
    {
        // Save current progress
        SaveGameProgress();

        // Clear session data
        GameSessionData.Instance.ClearSession();

        // Reset time scale
        Time.timeScale = 1f;

        // Mark game as inactive
        isGameActive = false;
        OnGameEnded?.Invoke();

        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
        Debug.Log("Returning to main menu");
    }

    private void SaveGameProgress()
    {
        if (!GameSessionData.Instance.IsSessionActive)
        {
            Debug.LogWarning("Cannot save progress: No active game session");
            return;
        }

        int saveSlot = GameSessionData.Instance.CurrentSaveSlot;
        int currentLevel = GameSessionData.Instance.CurrentLevel;
        CharacterType currentCharacter = GameSessionData.Instance.CurrentCharacter;

        // Save to PlayerPrefs (replace with your save system)
        string prefix = $"SaveSlot_{saveSlot}_";
        PlayerPrefs.SetInt(prefix + "IsOccupied", 1);
        PlayerPrefs.SetString(prefix + "PlayerName", $"Player {saveSlot + 1}");
        PlayerPrefs.SetInt(prefix + "LastLevel", currentLevel);
        PlayerPrefs.SetString(prefix + "LastCharacter", currentCharacter.ToString());

        PlayerPrefs.Save();

        Debug.Log($"Progress saved: Slot {saveSlot}, Level {currentLevel}, Character {currentCharacter}");
    }

    // Public methods for external systems
    public void CompleteLevel()
    {
        if (levelManager != null)
        {
            levelManager.CompleteLevel();
        }
    }

    public void LoadSpecificLevel(int levelNumber)
    {
        if (levelManager != null)
        {
            levelManager.LoadLevel(levelNumber);
        }
    }

    public LevelStats GetCurrentLevelStats()
    {
        return levelManager != null ? levelManager.GetLevelStats() : new LevelStats();
    }

    // Input handling
    private void Update()
    {
        // Handle pause input (ESC key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
}