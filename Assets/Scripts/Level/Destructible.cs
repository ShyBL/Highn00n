using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public GameObject tntPrefab;
    [Range(0f, 1f)] public float tntDropChance = 0.3f; // 30% chance to drop TNT

    public UnityEvent onDestroyed;

    private void SpawnPickup()
    {
        Instantiate(tntPrefab, transform.position, Quaternion.identity);
        
        // if (Random.value < tntDropChance)
        // {
        //     Instantiate(tntPrefab, transform.position, Quaternion.identity);
        //     Debug.Log("TNT Dropped!");
        // }
        // else
        // {
        //     Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        // }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            SpawnPickup();
            onDestroyed?.Invoke();
            Destroy(collision.gameObject); // Destroy bullet on impact
        }
    }
}
