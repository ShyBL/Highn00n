using UnityEngine;

/// <summary>
/// Singleton that manages game session data across scenes
/// Integrates with LevelManager to provide level information
/// </summary>
public class GameSessionData : MonoBehaviour
{
    private static GameSessionData _instance;
    public static GameSessionData Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSessionData");
                _instance = go.AddComponent<GameSessionData>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Current Session Data")]
    [SerializeField] private int currentSaveSlot = -1;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private CharacterType currentCharacter = CharacterType.Lloyd;
    [SerializeField] private bool isSessionActive = false;

    // Properties for easy access
    public int CurrentSaveSlot => currentSaveSlot;
    public int CurrentLevel => currentLevel;
    public CharacterType CurrentCharacter => currentCharacter;
    public bool IsSessionActive => isSessionActive;

    private void Awake()
    {
        // Ensure singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Initialize session data from the main menu
    /// </summary>
    public void Initialize(int saveSlot, int level, CharacterType character)
    {
        currentSaveSlot = saveSlot;
        currentLevel = level;
        currentCharacter = character;
        isSessionActive = true;

        Debug.Log($"Game session initialized: Slot {saveSlot}, Level {level}, Character {character}");
    }

    /// <summary>
    /// Update the current level (called when player progresses)
    /// </summary>
    public void SetCurrentLevel(int newLevel)
    {
        currentLevel = newLevel;
        Debug.Log($"Level updated to: {newLevel}");
    }

    /// <summary>
    /// Clear session data (called when returning to main menu)
    /// </summary>
    public void ClearSession()
    {
        currentSaveSlot = -1;
        currentLevel = 1;
        currentCharacter = CharacterType.Lloyd;
        isSessionActive = false;

        Debug.Log("Game session cleared");
    }

    /// <summary>
    /// Get save data for persistence
    /// </summary>
    public SaveSlotData GetCurrentSaveData()
    {
        if (!isSessionActive)
        {
            Debug.LogWarning("No active session to get save data from");
            return null;
        }

        return new SaveSlotData
        {
            isOccupied = true,
            playerName = $"Player {currentSaveSlot + 1}",
            lastPlayedLevel = currentLevel,
            lastUsedCharacter = currentCharacter,
            unlockedCharacters = GetUnlockedCharactersForSlot(currentSaveSlot)
        };
    }

    /// <summary>
    /// Helper method to get unlocked characters for current save slot
    /// This should be called from your save system
    /// </summary>
    private System.Collections.Generic.List<CharacterType> GetUnlockedCharactersForSlot(int slot)
    {
        // This is a placeholder - replace with your actual save system logic
        var unlockedChars = new System.Collections.Generic.List<CharacterType> { CharacterType.Lloyd };

        // Load from PlayerPrefs as fallback
        string prefix = $"SaveSlot_{slot}_";
        string unlockedCharsStr = PlayerPrefs.GetString(prefix + "UnlockedCharacters", "Lloyd");

        string[] charNames = unlockedCharsStr.Split(',');
        foreach (string charName in charNames)
        {
            if (System.Enum.TryParse(charName.Trim(), out CharacterType character))
            {
                if (!unlockedChars.Contains(character))
                {
                    unlockedChars.Add(character);
                }
            }
        }

        return unlockedChars;
    }
}