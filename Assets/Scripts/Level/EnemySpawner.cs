using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;

    public void SpawnZombie()
    {
        Instantiate(zombiePrefab, transform.position, Quaternion.identity);
    }
}