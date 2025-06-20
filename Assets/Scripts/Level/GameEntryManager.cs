using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveSlotData
{
    public bool isOccupied = false;
    public string playerName = "";
    public int lastPlayedLevel = 1;
    public CharacterType lastUsedCharacter = CharacterType.Lloyd;
    public List<CharacterType> unlockedCharacters = new List<CharacterType> { CharacterType.Lloyd };

    public SaveSlotData()
    {
        // Default constructor - Lloyd is unlocked by default
        unlockedCharacters.Add(CharacterType.Lloyd);
    }
}

public class GameEntryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject saveSlotPanel;
    public GameObject characterSelectionPanel;

    [Header("Save Slot UI")]
    public Button[] saveSlotButtons = new Button[3];
    public Text[] saveSlotTexts = new Text[3];

    [Header("Level Selection")]
    public Slider levelSlider;
    public Text levelText;

    [Header("Character Selection")]
    public Transform characterButtonContainer;
    public Button characterButtonPrefab;
    public Button confirmButton;
    public Text selectedCharacterText;

    [Header("Game Data")]
    public PlayerStats playerStats;
    public string gameSceneName = "GameScene";

    // Private variables
    private SaveSlotData[] saveSlots = new SaveSlotData[3];
    private int selectedSaveSlot = -1;
    private int selectedLevel = 1;
    private CharacterType selectedCharacter = CharacterType.Lloyd;
    private List<Button> characterButtons = new List<Button>();

    void Start()
    {
        InitializeSaveSlots();
        SetupUI();
        ShowMainMenu();
    }

    void InitializeSaveSlots()
    {
        // Initialize save slots with default data
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i] = new SaveSlotData();

            // For demo purposes, add some sample data to slots
            if (i == 0)
            {
                saveSlots[i].isOccupied = true;
                saveSlots[i].playerName = "Player 1";
                saveSlots[i].lastPlayedLevel = 15;
                saveSlots[i].lastUsedCharacter = CharacterType.Jack;
                saveSlots[i].unlockedCharacters.AddRange(new[] { CharacterType.Jack, CharacterType.Mai });
            }
            else if (i == 1)
            {
                saveSlots[i].isOccupied = true;
                saveSlots[i].playerName = "Player 2";
                saveSlots[i].lastPlayedLevel = 32;
                saveSlots[i].lastUsedCharacter = CharacterType.Dollothy;
                saveSlots[i].unlockedCharacters.AddRange(new[] { CharacterType.Jack, CharacterType.Dollothy, CharacterType.Antonio });
            }
            // Slot 2 remains empty for new game
        }

        // Load actual save data here if you have a save system
        LoadSaveData();
    }

    void SetupUI()
    {
        // Setup save slot buttons
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = i; // Capture for closure
            saveSlotButtons[i].onClick.AddListener(() => SelectSaveSlot(slotIndex));
            UpdateSaveSlotUI(i);
        }

        // Setup level slider
        levelSlider.minValue = 1;
        levelSlider.maxValue = 100;
        levelSlider.wholeNumbers = true;
        levelSlider.onValueChanged.AddListener(OnLevelChanged);

        // Setup confirm button
        confirmButton.onClick.AddListener(ConfirmSelection);

        UpdateLevelUI();
    }

    void UpdateSaveSlotUI(int slotIndex)
    {
        SaveSlotData slot = saveSlots[slotIndex];
        if (slot.isOccupied)
        {
            saveSlotTexts[slotIndex].text = $"{slot.playerName}\nLevel: {slot.lastPlayedLevel}\nCharacter: {slot.lastUsedCharacter}";
        }
        else
        {
            saveSlotTexts[slotIndex].text = "Empty Slot\n(New Game)";
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveSlotPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
    }

    public void ShowSaveSlotSelection()
    {
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(true);
        characterSelectionPanel.SetActive(false);
    }

    public void ShowCharacterSelection()
    {
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        characterSelectionPanel.SetActive(true);

        SetupCharacterButtons();
    }

    void SelectSaveSlot(int slotIndex)
    {
        selectedSaveSlot = slotIndex;
        SaveSlotData selectedSlot = saveSlots[slotIndex];

        // Set level slider to last played level or 1 for new games
        selectedLevel = selectedSlot.isOccupied ? selectedSlot.lastPlayedLevel : 1;
        levelSlider.value = selectedLevel;

        // Set default character
        selectedCharacter = selectedSlot.lastUsedCharacter;

        UpdateLevelUI();
        ShowCharacterSelection();
    }

    void OnLevelChanged(float value)
    {
        selectedLevel = (int)value;
        UpdateLevelUI();
    }

    void UpdateLevelUI()
    {
        levelText.text = $"Level: {selectedLevel}";
    }

    void SetupCharacterButtons()
    {
        // Clear existing buttons
        foreach (Button btn in characterButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        characterButtons.Clear();

        // Get available characters for selected save slot
        List<CharacterType> availableCharacters = GetAvailableCharacters(selectedSaveSlot);

        // Create character selection buttons
        foreach (CharacterType character in availableCharacters)
        {
            Button charButton = Instantiate(characterButtonPrefab, characterButtonContainer);
            charButton.GetComponentInChildren<Text>().text = character.ToString();

            // Capture character for closure
            CharacterType capturedChar = character;
            charButton.onClick.AddListener(() => SelectCharacter(capturedChar));

            characterButtons.Add(charButton);
        }

        // Auto-select first available character if current selection isn't available
        if (!availableCharacters.Contains(selectedCharacter))
        {
            selectedCharacter = availableCharacters[0];
        }

        UpdateCharacterSelectionUI();
    }

    List<CharacterType> GetAvailableCharacters(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= saveSlots.Length)
        {
            return new List<CharacterType> { CharacterType.Lloyd };
        }

        return saveSlots[slotIndex].unlockedCharacters;
    }

    void SelectCharacter(CharacterType character)
    {
        selectedCharacter = character;
        UpdateCharacterSelectionUI();
    }

    void UpdateCharacterSelectionUI()
    {
        selectedCharacterText.text = $"Selected: {selectedCharacter}";

        // Update button visual states
        foreach (Button btn in characterButtons)
        {
            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText.text == selectedCharacter.ToString())
            {
                btn.GetComponent<Image>().color = Color.yellow; // Highlight selected
            }
            else
            {
                btn.GetComponent<Image>().color = Color.white; // Normal state
            }
        }
    }

    void ConfirmSelection()
    {
        if (selectedSaveSlot < 0)
        {
            Debug.LogError("No save slot selected!");
            return;
        }

        // Update save slot data
        SaveSlotData selectedSlot = saveSlots[selectedSaveSlot];
        selectedSlot.isOccupied = true;
        selectedSlot.lastPlayedLevel = selectedLevel;
        selectedSlot.lastUsedCharacter = selectedCharacter;

        if (string.IsNullOrEmpty(selectedSlot.playerName))
        {
            selectedSlot.playerName = $"Player {selectedSaveSlot + 1}";
        }

        // Set the character in PlayerStats
        if (playerStats != null)
        {
            playerStats.ChangeCharacter(selectedCharacter);
        }

        // Save the game data
        SaveGameData();

        // Load the game scene
        LoadGameScene();
    }

    void LoadGameScene()
    {
        // Pass game data to the next scene
        GameSessionData.Instance.Initialize(selectedSaveSlot, selectedLevel, selectedCharacter);

        Debug.Log($"Loading game: Slot {selectedSaveSlot}, Level {selectedLevel}, Character {selectedCharacter}");

        SceneManager.LoadScene(gameSceneName);
    }

    void SaveGameData()
    {
        // Simple PlayerPrefs save system - replace with your preferred save system
        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveSlotData slot = saveSlots[i];
            string prefix = $"SaveSlot_{i}_";

            PlayerPrefs.SetInt(prefix + "IsOccupied", slot.isOccupied ? 1 : 0);
            PlayerPrefs.SetString(prefix + "PlayerName", slot.playerName);
            PlayerPrefs.SetInt(prefix + "LastLevel", slot.lastPlayedLevel);
            PlayerPrefs.SetString(prefix + "LastCharacter", slot.lastUsedCharacter.ToString());

            // Save unlocked characters as a comma-separated string
            string unlockedChars = string.Join(",", slot.unlockedCharacters);
            PlayerPrefs.SetString(prefix + "UnlockedCharacters", unlockedChars);
        }

        PlayerPrefs.Save();
    }

    void LoadSaveData()
    {
        // Load save data from PlayerPrefs
        for (int i = 0; i < saveSlots.Length; i++)
        {
            string prefix = $"SaveSlot_{i}_";

            if (PlayerPrefs.HasKey(prefix + "IsOccupied"))
            {
                SaveSlotData slot = saveSlots[i];
                slot.isOccupied = PlayerPrefs.GetInt(prefix + "IsOccupied") == 1;
                slot.playerName = PlayerPrefs.GetString(prefix + "PlayerName", "");
                slot.lastPlayedLevel = PlayerPrefs.GetInt(prefix + "LastLevel", 1);

                // Load last used character
                string lastCharStr = PlayerPrefs.GetString(prefix + "LastCharacter", "Lloyd");
                if (System.Enum.TryParse(lastCharStr, out CharacterType lastChar))
                {
                    slot.lastUsedCharacter = lastChar;
                }

                // Load unlocked characters
                string unlockedCharsStr = PlayerPrefs.GetString(prefix + "UnlockedCharacters", "Lloyd");
                slot.unlockedCharacters.Clear();

                string[] charNames = unlockedCharsStr.Split(',');
                foreach (string charName in charNames)
                {
                    if (System.Enum.TryParse(charName.Trim(), out CharacterType character))
                    {
                        if (!slot.unlockedCharacters.Contains(character))
                        {
                            slot.unlockedCharacters.Add(character);
                        }
                    }
                }

                // Ensure Lloyd is always unlocked
                if (!slot.unlockedCharacters.Contains(CharacterType.Lloyd))
                {
                    slot.unlockedCharacters.Add(CharacterType.Lloyd);
                }
            }
        }
    }

    // Helper method to unlock characters (call this from gameplay)
    public void UnlockCharacter(int saveSlot, CharacterType character)
    {
        if (saveSlot >= 0 && saveSlot < saveSlots.Length)
        {
            if (!saveSlots[saveSlot].unlockedCharacters.Contains(character))
            {
                saveSlots[saveSlot].unlockedCharacters.Add(character);
                SaveGameData();
                Debug.Log($"Unlocked character {character} for save slot {saveSlot}");
            }
        }
    }

    // UI Button callbacks
    public void OnNewGameClicked()
    {
        ShowSaveSlotSelection();
    }

    public void OnBackToMainMenuClicked()
    {
        ShowMainMenu();
    }

    public void OnBackToSaveSlotClicked()
    {
        ShowSaveSlotSelection();
    }

    public void OnQuitGameClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}