using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Level Integration")]
    [SerializeField] private LevelManager levelManager;
    
    private void Start()
    {
        // Auto-find LevelManager if not assigned
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
    }

    public void SpawnZombie()
    {
        Instantiate(zombiePrefab, transform.position, Quaternion.identity);
    }
    
    /// <summary>
    /// Spawn multiple zombies based on level scaling
    /// </summary>
    public void SpawnZombieWave()
    {
        if (levelManager != null && levelManager.LevelConfig != null)
        {
            int zombieCount = levelManager.LevelConfig.GetZombieCount(levelManager.CurrentLevel);
            StartCoroutine(SpawnZombieWaveCoroutine(zombieCount));
        }
        else
        {
            // Fallback: spawn 3 zombies
            StartCoroutine(SpawnZombieWaveCoroutine(3));
        }
    }
    
    private IEnumerator SpawnZombieWaveCoroutine(int zombieCount)
    {
        Debug.Log($"Spawning wave of {zombieCount} zombies...");
        
        for (int i = 0; i < zombieCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            
            yield return new WaitForSeconds(spawnDelay);
        }
        
        Debug.Log($"Zombie wave complete! {zombieCount} zombies spawned.");
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // Use spawn points if available
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return randomSpawnPoint.position;
        }
        else
        {
            // Use spawner position with some randomness
            Vector3 randomOffset = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f),
                0f
            );
            return transform.position + randomOffset;
        }
    }
}