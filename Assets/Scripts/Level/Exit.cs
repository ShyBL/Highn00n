using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Exit : MonoBehaviour
{
    [Header("Level Integration")]
    [SerializeField] private LevelManager levelManager;
    
    [Header("Events")]
    public UnityEvent onCanExit;

    private void Start()
    {
        // Auto-find LevelManager if not assigned
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BombPlanting bombPlanting))
        {
            if (bombPlanting.allClockTowers.All(tower => tower.hasBombPlanted))
            {
                // Trigger level completion through LevelManager
                if (levelManager != null)
                {
                    levelManager.CompleteLevel();
                    Debug.Log("Level completed! All bombs planted.");
                }
                else
                {
                    Debug.LogWarning("LevelManager not found! Level completion not triggered.");
                }
                
                onCanExit?.Invoke();
            }
        }
    }
}