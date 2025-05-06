using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private LevelScalingConfig levelConfig;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float minDistanceBetweenObjects = 2f;
    
    [Header("Placeable Objects")]
    [SerializeField] private List<PlaceableObjectConfig> placeableObjects = new();
    
    private Vector2 mapSize;
    private List<Vector3> occupiedPositions = new();
    private Transform objectContainer;
    private IObjectPlacer objectPlacer;

    private void Awake()
    {
        CreateObjectContainer();
        SetMapSizeFromTilemap();
        InitializeObjectPlacer();
    }

    private void Start()
    {
        PlaceAllObjects();
    }

    private void InitializeObjectPlacer()
    {
        objectPlacer = new ZoneBasedPlacer(
            referenceTilemap, 
            mapSize, 
            occupiedPositions, 
            minDistanceBetweenObjects
        );
    }
    
    private void PlaceAllObjects()
    {
        int totalObjectsPlaced = 0;
        Dictionary<PlacementZone, int> zoneStats = new Dictionary<PlacementZone, int>();
        
        foreach (var objectConfig in placeableObjects)
        {
            int quantity = objectConfig.GetQuantity(currentLevel, levelConfig);
            PlacementZone zone = objectConfig.Zone;
            
            // Track statistics by zone
            if (!zoneStats.ContainsKey(zone))
                zoneStats[zone] = 0;
            
            for (int i = 0; i < quantity; i++)
            {
                objectPlacer.PlaceObject(objectConfig.Prefab, zone, objectContainer);
                totalObjectsPlaced++;
                zoneStats[zone]++;
            }
        }
        
        // Log placement results with zone details
        Debug.Log($"Level {currentLevel}: Placed {totalObjectsPlaced} objects in total");
        foreach (var zoneStat in zoneStats)
        {
            Debug.Log($"- {zoneStat.Key}: {zoneStat.Value} objects");
        }
    }

    private void CreateObjectContainer()
    {
        var container = new GameObject("LevelObjects_Container");
        container.transform.parent = referenceTilemap.transform.parent;
        objectContainer = container.transform;
    }

    private void SetMapSizeFromTilemap()
    {
        referenceTilemap.CompressBounds();
        var bounds = referenceTilemap.cellBounds;
        mapSize = new Vector2(bounds.size.x, bounds.size.y);
        
        Debug.Log($"Map size set to {mapSize.x} x {mapSize.y} based on tilemap");
    }
}