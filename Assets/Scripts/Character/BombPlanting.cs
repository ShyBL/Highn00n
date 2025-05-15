using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class ClockTowerEvent : UnityEvent<ClockMechanic> { }

public class BombPlanting : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private KeyCode plantKey = KeyCode.E;
    [SerializeField] private GameObject bombPrefab;
    
    [Header("Events")]
    public UnityEvent onBombPickup;
    public ClockTowerEvent onBombPlant = new ClockTowerEvent();
    
    private bool hasBomb = false;
    public List<ClockMechanic> allClockTowers = new List<ClockMechanic>();
    private ClockMechanic activeClockTower = null;
    
    void Update()
    {
        if (hasBomb && activeClockTower != null && Input.GetKeyDown(plantKey))
        {
            PlantBomb(activeClockTower);
        }
    }
    
    void PlantBomb(ClockMechanic targetClockTower)
    {
        if (!hasBomb || targetClockTower == null) return;
        
        GameObject plantedBomb = Instantiate(bombPrefab, targetClockTower.transform.position, Quaternion.identity);
        plantedBomb.transform.SetParent(targetClockTower.transform);
        
        // Pass the specific clock tower to the event
        onBombPlant.Invoke(targetClockTower);
        
        Debug.Log("Bomb planted! Find the exit!");
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ClockMechanic clockMechanic))
        {
            activeClockTower = clockMechanic;
            Debug.Log("Near a clock tower. Press " + plantKey + " to plant the bomb.");
        }
        
        // Check for bomb pickup
        if (collision.TryGetComponent(out Bomb bomb) && !hasBomb)
        {
            hasBomb = true;
            
            onBombPickup.Invoke();
            
            Destroy(collision.gameObject);
            Debug.Log("Bomb picked up!");
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ClockMechanic clockMechanic) && 
            activeClockTower == clockMechanic)
        {
            activeClockTower = null;
        }
    }
}