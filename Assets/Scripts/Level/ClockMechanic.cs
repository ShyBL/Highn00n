using UnityEngine;
using UnityEngine.Events;

public class ClockMechanic : MonoBehaviour
{
    public float timeLimit = 12f; // 12 seconds per countdown
    public UnityEvent onClockStrikeHighNoon; // Event for zombie spawn
    private float timer;
    private bool highNoonTriggered = false;

    [SerializeField] private bool hasBombPlanted = false;
    public UnityEvent onBombPlanted;
    
    // Called when a bomb is planted on this clock tower
    public void OnBombPlanted()
    {
        hasBombPlanted = true;
        onBombPlanted?.Invoke();
        
        // You could trigger effects, animations, or countdown here
    }
    
    public bool HasBombPlanted()
    {
        return hasBombPlanted;
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