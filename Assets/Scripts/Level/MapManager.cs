using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject tntPrefab;
    public GameObject clockTowerPrefab;
    public GameObject exitPrefab;

    public Vector2 mapSize;

    void Start()
    {
        PlaceObjectRandomly(tntPrefab);
        PlaceObjectRandomly(clockTowerPrefab);
        PlaceObjectRandomly(exitPrefab);
    }

    void PlaceObjectRandomly(GameObject obj)
    {
        Vector2 randomPosition = new Vector2(
            Random.Range(-mapSize.x / 2, mapSize.x / 2),
            Random.Range(-mapSize.y / 2, mapSize.y / 2)
        );
        Instantiate(obj, randomPosition, Quaternion.identity);
    }
}