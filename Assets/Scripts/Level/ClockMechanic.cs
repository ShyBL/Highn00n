using UnityEngine;
using UnityEngine.Events;

public class ClockMechanic : MonoBehaviour
{
    public float timeLimit = 12f; // 12 seconds per countdown
    public UnityEvent onClockStrikeHighNoon; // Event for zombie spawn
    public UnityEvent onTimeLimitReached; // Event for exit phase
    private float timer;
    private bool highNoonTriggered = false;

    void Start()
    {
        timer = timeLimit; // Initialize the timer
    }

    void Update()
    {
        // Countdown logic
        timer -= Time.deltaTime;

        if (timer <= 0f && !highNoonTriggered)
        {
            // Trigger High Noon event (zombie summoning)
            highNoonTriggered = true;
            onClockStrikeHighNoon.Invoke();

            // Restart timer for next cycle
            timer = timeLimit;
            highNoonTriggered = false;
        }
    }

    public void TriggerExitPhase()
    {
        // Trigger exit phase (after clock towers are destroyed)
        onTimeLimitReached.Invoke();
    }

    // For debugging purposes, show the timer in the console
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Time Left: " + Mathf.Ceil(timer) + " seconds");
    }
}