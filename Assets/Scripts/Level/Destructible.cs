using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour
{
    public GameObject powerUpPrefab; // Power-up spawn
    public GameObject tntPrefab; // TNT spawn
    [Range(0f, 1f)] public float tntDropChance = 0.3f; // 30% chance to drop TNT

    public UnityEvent onDestroyed; // Event triggered when destroyed

    private void TakeDamage()
    {
        onDestroyed?.Invoke(); // Trigger Unity event

        Destroy(gameObject);
        

        if (Random.value < tntDropChance)
        {
            Instantiate(tntPrefab, transform.position, Quaternion.identity);
            Debug.Log("TNT Dropped!");
        }
        else
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            TakeDamage();
            Destroy(collision.gameObject); // Destroy bullet on impact
        }
    }
}
