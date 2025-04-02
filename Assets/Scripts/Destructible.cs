using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject powerUpPrefab; // TNT or other power-ups

    public void TakeDamage()
    {
        Destroy(gameObject);
        Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
    }
}