using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 12f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnZombie();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnZombie()
    {
        foreach (var point in spawnPoints)
        {
            Instantiate(zombiePrefab, point.position, Quaternion.identity);
        }
    }
}