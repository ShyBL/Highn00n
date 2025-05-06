using System;
using UnityEngine;
using UnityEngine.Events;

public class ClockMechanic : MonoBehaviour
{
    public float timeLimit = 12f; // 12 seconds per countdown
    public UnityEvent onClockStrikeHighNoon; // Event for zombie spawn
    public UnityEvent onBombPlanted;
    private float timer;
    private bool highNoonTriggered = false;
    public bool hasBombPlanted = false;
    
    private void Awake()
    {
        FindFirstObjectByType<BombPlanting>().onBombPlant.AddListener(OnBombPlanted);
    }

    // Called when a bomb is planted on this clock tower
    private void OnBombPlanted()
    {
        hasBombPlanted = true;
        onBombPlanted?.Invoke();
        
        // You could trigger effects, animations, or countdown here
    }
    
    private void Start()
    {
        timer = timeLimit;
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

            // Restart timer for next cycle
            timer = timeLimit;
            highNoonTriggered = false;
        }
    }
}