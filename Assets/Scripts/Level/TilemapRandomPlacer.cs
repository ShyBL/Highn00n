using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IObjectPlacer
{
    void PlaceObject(GameObject prefab, PlacementZone zone, Transform parent);
}

public class ZoneBasedPlacer : IObjectPlacer
{
    private readonly Tilemap tilemap;
    private readonly Vector2 mapSize;
    private readonly List<Vector3> occupiedPositions;
    private readonly float minDistance;
    private readonly int maxAttempts;

    public ZoneBasedPlacer(Tilemap tilemap, Vector2 mapSize, List<Vector3> occupiedPositions, 
                          float minDistance = 2f, int maxAttempts = 50)
    {
        this.tilemap = tilemap;
        this.mapSize = mapSize;
        this.occupiedPositions = occupiedPositions;
        this.minDistance = minDistance;
        this.maxAttempts = maxAttempts;
    }

    public void PlaceObject(GameObject prefab, PlacementZone zone, Transform parent)
    {
        Vector3 position = GetValidPositionInZone(zone);
        
        var spawnedObj = GameObject.Instantiate(prefab, position, Quaternion.identity);
        spawnedObj.transform.parent = parent;
        
        occupiedPositions.Add(position);
    }

    private Vector3 GetValidPositionInZone(PlacementZone zone)
    {
        Vector3 snappedPosition;
        int attempts = 0;
        
        do
        {
            var cellMin = tilemap.cellBounds.min;
            var worldMin = tilemap.CellToWorld(cellMin);
            
            float randX, randY;
            
            // Apply zone-specific constraints
            switch (zone)
            {
                case PlacementZone.TopArea:
                    // Top 25% of the map
                    randX = UnityEngine.Random.Range(0, mapSize.x);
                    randY = UnityEngine.Random.Range(mapSize.y * 0.75f, mapSize.y);
                    break;
                    
                case PlacementZone.BottomArea:
                    // Bottom 25% of the map
                    randX = UnityEngine.Random.Range(0, mapSize.x);
                    randY = UnityEngine.Random.Range(0, mapSize.y * 0.25f);
                    break;
                    
                case PlacementZone.LeftSide:
                    // Left 25% of the map
                    randX = UnityEngine.Random.Range(0, mapSize.x * 0.25f);
                    randY = UnityEngine.Random.Range(0, mapSize.y);
                    break;
                    
                case PlacementZone.RightSide:
                    // Right 25% of the map
                    randX = UnityEngine.Random.Range(mapSize.x * 0.75f, mapSize.x);
                    randY = UnityEngine.Random.Range(0, mapSize.y);
                    break;
                    
                case PlacementZone.Center:
                    // Center 50% of the map
                    randX = UnityEngine.Random.Range(mapSize.x * 0.25f, mapSize.x * 0.75f);
                    randY = UnityEngine.Random.Range(mapSize.y * 0.25f, mapSize.y * 0.75f);
                    break;
                    
                case PlacementZone.Anywhere:
                default:
                    // Full map
                    randX = UnityEngine.Random.Range(0, mapSize.x);
                    randY = UnityEngine.Random.Range(0, mapSize.y);
                    break;
            }
            
            var worldPosition = worldMin + new Vector3(randX, randY, 0);
            
            var cell = tilemap.WorldToCell(worldPosition);
            snappedPosition = tilemap.GetCellCenterWorld(cell);
            
            attempts++;
            
            if (attempts >= maxAttempts)
                break;
                
        } while (IsPositionOccupied(snappedPosition));
        
        return snappedPosition;
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        foreach (var occupiedPos in occupiedPositions)
        {
            if (Vector3.Distance(position, occupiedPos) < minDistance)
                return true;
        }
        return false;
    }
}