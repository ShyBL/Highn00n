using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColliderBasedPlacer : IObjectPlacer
{
    private readonly Tilemap tilemap;
    private readonly Vector2 mapSize;
    private readonly List<Vector3> occupiedPositions;
    private readonly float minDistance;
    private readonly int maxAttempts;

    public ColliderBasedPlacer(Tilemap tilemap, Vector2 mapSize, List<Vector3> occupiedPositions, 
        float minDistance = 2f, int maxAttempts = 50)
    {
        this.tilemap = tilemap;
        this.mapSize = mapSize;
        this.occupiedPositions = occupiedPositions;
        this.minDistance = minDistance;
        this.maxAttempts = maxAttempts;
    }

    public void PlaceObject(GameObject prefab, IPlacementBoundary boundary, Transform parent)
    {
        Vector3 position = GetValidPosition(boundary);
        
        var spawnedObj = GameObject.Instantiate(prefab, position, Quaternion.identity);
        spawnedObj.transform.parent = parent;
        
        occupiedPositions.Add(position);
    }

    private Vector3 GetValidPosition(IPlacementBoundary boundary)
    {
        Vector3 snappedPosition;
        int attempts = 0;
        
        do
        {
            var cellMin = tilemap.cellBounds.min;
            var worldMin = tilemap.CellToWorld(cellMin);
            
            // Get a random position within the boundary
            var worldPosition = boundary.GetRandomPointWithinBoundary(worldMin, mapSize);
            
            // Snap to grid
            var cell = tilemap.WorldToCell(worldPosition);
            snappedPosition = tilemap.GetCellCenterWorld(cell);
            
            attempts++;
            
            if (attempts >= maxAttempts)
                break;
                
        } while (IsPositionOccupied(snappedPosition) || !boundary.IsPointWithinBoundary(snappedPosition));
        
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