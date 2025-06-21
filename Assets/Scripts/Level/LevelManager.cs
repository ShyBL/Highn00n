using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Enhanced LevelManager that integrates with GameSessionData and supports level progression
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private LevelScalingConfig levelConfig;
    [SerializeField] private PlayerStats playerStats;
    
    [Header("Level Management")]
    [SerializeField] private int fallbackLevel = 1; // Used if no session data available
    [SerializeField] private float minDistanceBetweenObjects = 2f;
    
    [Header("Placeable Objects")]
    [SerializeField] private List<PlaceableObjectConfig> placeableObjects = new();
    
    [Header("Default Placement Area")]
    [SerializeField] private Collider2D defaultPlacementArea;
    
    [Header("Level Progression")]
    [SerializeField] private bool autoProgressToNextLevel = true;
    [SerializeField] private float levelCompleteDelay = 2f;
    
    // Private variables
    private int currentLevel;
    private Vector2 mapSize;
    private List<Vector3> occupiedPositions = new();
    private Transform objectContainer;
    private IObjectPlacer objectPlacer;
    private ColliderBoundary defaultBoundary = new ColliderBoundary();
    
    // Events
    public System.Action<int> OnLevelStarted;
    public System.Action<int> OnLevelCompleted;
    public System.Action<int, int> OnLevelProgressChanged; // current, max levels
    
    // Properties
    public int CurrentLevel => currentLevel;
    public LevelScalingConfig LevelConfig => levelConfig;
    
    private void Awake()
    {
        InitializeLevel();
        CreateObjectContainer();
        SetMapSizeFromTilemap();
        InitializeObjectPlacer();
        SetupPlayerCharacter();
    }
    
    private void Start()
    {
        PlaceAllObjects();
        OnLevelStarted?.Invoke(currentLevel);
    }
    
    private void InitializeLevel()
    {
        // Get level from GameSessionData if available, otherwise use fallback
        if (GameSessionData.Instance.IsSessionActive)
        {
            currentLevel = GameSessionData.Instance.CurrentLevel;
            Debug.Log($"Level loaded from session data: {currentLevel}");
        }
        else
        {
            currentLevel = fallbackLevel;
            Debug.LogWarning($"No session data found, using fallback level: {currentLevel}");
        }
        
        // Ensure level is within valid range
        currentLevel = Mathf.Clamp(currentLevel, 1, 100);
    }
    
    private void SetupPlayerCharacter()
    {
        if (playerStats != null && GameSessionData.Instance.IsSessionActive)
        {
            CharacterType sessionCharacter = GameSessionData.Instance.CurrentCharacter;
            if (playerStats.CurrentCharacter != sessionCharacter)
            {
                playerStats.ChangeCharacter(sessionCharacter);
                Debug.Log($"Player character set to: {sessionCharacter}");
            }
        }
    }
    
    private void InitializeObjectPlacer()
    {
        objectPlacer = new ColliderBasedPlacer(
            referenceTilemap, 
            mapSize, 
            occupiedPositions, 
            minDistanceBetweenObjects
        );
        
        // Set up default boundary using reflection (as in your original code)
        var boundaryField = defaultBoundary.GetType().GetField("boundaryCollider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (boundaryField != null)
        {
            boundaryField.SetValue(defaultBoundary, defaultPlacementArea);
        }
    }
    
    private void PlaceAllObjects()
    {
        // Clear any existing objects
        ClearLevelObjects();
        occupiedPositions.Clear();
        
        int totalObjectsPlaced = 0;
        Dictionary<string, int> objectStats = new Dictionary<string, int>();
        
        foreach (var objectConfig in placeableObjects)
        {
            int quantity = objectConfig.GetQuantity(currentLevel, levelConfig);
            string objectName = objectConfig.Prefab?.name ?? "Unknown";
            
            // Track statistics by object type
            if (!objectStats.ContainsKey(objectName))
                objectStats[objectName] = 0;
            
            // Use either the object's specific boundary or the default boundary
            IPlacementBoundary boundary = objectConfig.Boundary.BoundaryCollider != null ? 
                                         objectConfig.Boundary : defaultBoundary;
            
            for (int i = 0; i < quantity; i++)
            {
                objectPlacer.PlaceObject(objectConfig.Prefab, boundary, objectContainer);
                totalObjectsPlaced++;
                objectStats[objectName]++;
            }
        }
        
        // Log placement results with object details
        Debug.Log($"Level {currentLevel}: Placed {totalObjectsPlaced} objects in total");
        foreach (var stat in objectStats)
        {
            Debug.Log($"- {stat.Key}: {stat.Value} objects");
        }
    }
    
    private void ClearLevelObjects()
    {
        if (objectContainer != null)
        {
            for (int i = objectContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(objectContainer.GetChild(i).gameObject);
            }
        }
    }
    
    private void CreateObjectContainer()
    {
        // Remove existing container if it exists
        if (objectContainer != null)
        {
            DestroyImmediate(objectContainer.gameObject);
        }
        
        var container = new GameObject($"LevelObjects_Container_L{currentLevel}");
        container.transform.parent = referenceTilemap.transform.parent;
        objectContainer = container.transform;
    }
    
    private void SetMapSizeFromTilemap()
    {
        referenceTilemap.CompressBounds();
        var bounds = referenceTilemap.cellBounds;
        mapSize = new Vector2(bounds.size.x, bounds.size.y);
        
        Debug.Log($"Map size set to {mapSize.x} x {mapSize.y} based on tilemap");
    }
    
    /// <summary>
    /// Call this when the player completes the current level
    /// </summary>
    public void CompleteLevel()
    {
        Debug.Log($"Level {currentLevel} completed!");
        OnLevelCompleted?.Invoke(currentLevel);
        
        if (autoProgressToNextLevel)
        {
            Invoke(nameof(ProgressToNextLevel), levelCompleteDelay);
        }
    }
    
    /// <summary>
    /// Progress to the next level
    /// </summary>
    public void ProgressToNextLevel()
    {
        if (currentLevel < 100) // Max level check
        {
            currentLevel++;
            
            // Update session data
            if (GameSessionData.Instance.IsSessionActive)
            {
                GameSessionData.Instance.SetCurrentLevel(currentLevel);
            }
            
            // Regenerate level
            RegenerateLevel();
            
            OnLevelStarted?.Invoke(currentLevel);
            OnLevelProgressChanged?.Invoke(currentLevel, 100);
        }
        else
        {
            Debug.Log("Maximum level reached!");
            // Handle game completion logic here
        }
    }
    
    /// <summary>
    /// Regenerate the current level (useful for level restarts or progression)
    /// </summary>
    public void RegenerateLevel()
    {
        CreateObjectContainer();
        PlaceAllObjects();
        Debug.Log($"Level {currentLevel} regenerated");
    }
    
    /// <summary>
    /// Load a specific level (useful for level select or debugging)
    /// </summary>
    public void LoadLevel(int levelNumber)
    {
        levelNumber = Mathf.Clamp(levelNumber, 1, 100);
        currentLevel = levelNumber;
        
        // Update session data
        if (GameSessionData.Instance.IsSessionActive)
        {
            GameSessionData.Instance.SetCurrentLevel(currentLevel);
        }
        
        RegenerateLevel();
        OnLevelStarted?.Invoke(currentLevel);
        OnLevelProgressChanged?.Invoke(currentLevel, 100);
    }
    
    /// <summary>
    /// Get level statistics for UI display
    /// </summary>
    public LevelStats GetLevelStats()
    {
        return new LevelStats
        {
            currentLevel = currentLevel,
            powerUpCrates = levelConfig.GetPowerUpCrateCount(currentLevel),
            clockTowers = levelConfig.GetClockTowerCount(currentLevel),
            zombiesPerWave = levelConfig.GetZombieCount(currentLevel),
            totalObjects = GetTotalObjectCount()
        };
    }
    
    private int GetTotalObjectCount()
    {
        int total = 0;
        foreach (var objectConfig in placeableObjects)
        {
            total += objectConfig.GetQuantity(currentLevel, levelConfig);
        }
        return total;
    }
    
    /// <summary>
    /// Reset level to initial state
    /// </summary>
    public void RestartLevel()
    {
        RegenerateLevel();
        OnLevelStarted?.Invoke(currentLevel);
    }
    
    // Editor/Debug methods
    [ContextMenu("Regenerate Current Level")]
    private void RegenerateLevelEditor()
    {
        if (Application.isPlaying)
        {
            RegenerateLevel();
        }
    }
    
    [ContextMenu("Progress to Next Level")]
    private void ProgressToNextLevelEditor()
    {
        if (Application.isPlaying)
        {
            ProgressToNextLevel();
        }
    }
}

/// <summary>
/// Data structure for level statistics
/// </summary>
[System.Serializable]
public struct LevelStats
{
    public int currentLevel;
    public int powerUpCrates;
    public int clockTowers;
    public int zombiesPerWave;
    public int totalObjects;
}