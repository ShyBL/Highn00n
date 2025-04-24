using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BombPlanting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KeyCode plantKey = KeyCode.E;
    [SerializeField] private GameObject bombPrefab;
    
    [Header("Events")]
    public UnityEvent onBombPickup;
    public UnityEvent onBombPlant;
    
    private bool hasBomb = false;
    private List<ClockMechanic> allClockTowers = new List<ClockMechanic>();
    private ClockMechanic activeClockTower = null;

    private void Awake()
    {
        // Find all clock towers in the scene at startup
        ClockMechanic[] clockTowers = FindObjectsOfType<ClockMechanic>();
        allClockTowers.AddRange(clockTowers);
        
        Debug.Log($"Found {allClockTowers.Count} clock towers in this level");
    }

    void Update()
    {
        if (hasBomb && activeClockTower != null && Input.GetKeyDown(plantKey))
        {
            PlantBomb(activeClockTower.transform);
        }
    }

    void PlantBomb(Transform targetTransform)
    {
        if (!hasBomb || targetTransform == null) return;
        
        GameObject plantedBomb = Instantiate(bombPrefab, targetTransform.position, Quaternion.identity);
        plantedBomb.transform.SetParent(targetTransform);
        
        onBombPlant.Invoke();
        
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
        if (collision.TryGetComponent<ClockMechanic>(out ClockMechanic clockMechanic) && 
            activeClockTower == clockMechanic)
        {
            activeClockTower = null;
        }
    }
}