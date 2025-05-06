using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public int currentLevel = 1;
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private LevelScalingConfig levelConfig;
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
        objectPlacer = new TilemapRandomPlacer(
            referenceTilemap, 
            mapSize, 
            occupiedPositions, 
            minDistanceBetweenObjects
        );
    }
    
    private void PlaceAllObjects()
    {
        int totalObjectsPlaced = 0;
        
        foreach (var objectConfig in placeableObjects)
        {
            int quantity = objectConfig.GetQuantity(currentLevel, levelConfig);
            
            for (int i = 0; i < quantity; i++)
            {
                objectPlacer.PlaceObject(objectConfig.Prefab, objectContainer);
                totalObjectsPlaced++;
            }
        }
        
        Debug.Log($"Level {currentLevel}: Placed {totalObjectsPlaced} objects in total");
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