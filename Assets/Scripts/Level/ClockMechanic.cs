using System;
using UnityEngine;
using UnityEngine.Events;

public class ClockMechanic : MonoBehaviour 
{
    [Header("Timer Settings")]
    public float timeLimit = 12f; // 12 seconds per countdown
    
    [Header("Level Integration")]
    [SerializeField] private LevelManager levelManager;
    
    [Header("Events")]
    public UnityEvent onClockStrikeHighNoon; // Event for zombie spawn
    
    private float timer;
    private bool highNoonTriggered = false;
    public bool hasBombPlanted = false;
    
    private void Awake()
    {
        var bombPlanting = FindFirstObjectByType<BombPlanting>();
        bombPlanting.onBombPlant.AddListener(OnBombPlantedAt);
        bombPlanting.allClockTowers.Add(this);
    }
    
    private void Start()
    {
        // Auto-find LevelManager if not assigned
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
            
        timer = timeLimit;
    }
    
    private void OnBombPlantedAt(ClockMechanic clockTower)
    {
        if (clockTower == this)
        {
            hasBombPlanted = true;
        }
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        
        TriggerZombieSpawn();
    }
    
    private void TriggerZombieSpawn()
    {
        if (timer <= 0f && !highNoonTriggered && !hasBombPlanted)
        {
            // Trigger High Noon event (zombie summoning)
            highNoonTriggered = true;
            onClockStrikeHighNoon.Invoke();
            
            // Log zombie spawn with level info
            if (levelManager != null)
            {
                int zombieCount = levelManager.LevelConfig.GetZombieCount(levelManager.CurrentLevel);
                Debug.Log($"High Noon! Spawning {zombieCount} zombies for level {levelManager.CurrentLevel}");
            }
            else
            {
                Debug.Log("High Noon! Zombies spawning (LevelManager not found for count info)");
            }
            
            // Restart timer for next cycle
            timer = timeLimit;
            highNoonTriggered = false;
        }
    }
}