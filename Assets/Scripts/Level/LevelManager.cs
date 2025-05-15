using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private LevelScalingConfig levelConfig;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float minDistanceBetweenObjects = 2f;
    
    [Header("Placeable Objects")]
    [SerializeField] private List<PlaceableObjectConfig> placeableObjects = new();
    
    [Header("Default Placement Area")]
    [SerializeField] private Collider2D defaultPlacementArea;
    
    private Vector2 mapSize;
    private List<Vector3> occupiedPositions = new();
    private Transform objectContainer;
    private IObjectPlacer objectPlacer;
    private ColliderBoundary defaultBoundary = new ColliderBoundary();

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
        objectPlacer = new ColliderBasedPlacer(
            referenceTilemap, 
            mapSize, 
            occupiedPositions, 
            minDistanceBetweenObjects
        );
        
        // Set up default boundary
        var boundaryField = defaultBoundary.GetType().GetField("boundaryCollider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (boundaryField != null)
        {
            boundaryField.SetValue(defaultBoundary, defaultPlacementArea);
        }
    }
    
    private void PlaceAllObjects()
    {
        int totalObjectsPlaced = 0;
        Dictionary<string, int> objectStats = new Dictionary<string, int>();
        
        foreach (var objectConfig in placeableObjects)
        {
            int quantity = objectConfig.GetQuantity(currentLevel, levelConfig);
            string objectName = objectConfig.Prefab?.name ?? "Unknown";
            
            // Track statistics by object type
            if (!objectStats.ContainsKey(objectName))
                objectStats[objectName] = 0;
            
            // Use either the object's specific boundary or the default boundary
            IPlacementBoundary boundary = objectConfig.Boundary.BoundaryCollider != null ? 
                                         objectConfig.Boundary : defaultBoundary;
            
            for (int i = 0; i < quantity; i++)
            {
                objectPlacer.PlaceObject(objectConfig.Prefab, boundary, objectContainer);
                totalObjectsPlaced++;
                objectStats[objectName]++;
            }
        }
        
        // Log placement results with object details
        Debug.Log($"Level {currentLevel}: Placed {totalObjectsPlaced} objects in total");
        foreach (var stat in objectStats)
        {
            Debug.Log($"- {stat.Key}: {stat.Value} objects");
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