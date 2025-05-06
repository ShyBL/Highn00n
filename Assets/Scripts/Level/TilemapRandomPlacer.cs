using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IObjectPlacer
{
    void PlaceObject(GameObject prefab, Transform parent);
}

public class TilemapRandomPlacer : IObjectPlacer
{
    private readonly Tilemap tilemap;
    private readonly Vector2 mapSize;
    private readonly List<Vector3> occupiedPositions;
    private readonly float minDistance;
    private readonly int maxAttempts;

    public TilemapRandomPlacer(Tilemap tilemap, Vector2 mapSize, List<Vector3> occupiedPositions, 
        float minDistance = 2f, int maxAttempts = 50)
    {
        this.tilemap = tilemap;
        this.mapSize = mapSize;
        this.occupiedPositions = occupiedPositions;
        this.minDistance = minDistance;
        this.maxAttempts = maxAttempts;
    }

    public void PlaceObject(GameObject prefab, Transform parent)
    {
        Vector3 position = GetValidPosition();
        
        var spawnedObj = GameObject.Instantiate(prefab, position, Quaternion.identity);
        spawnedObj.transform.parent = parent;
        
        occupiedPositions.Add(position);
    }

    private Vector3 GetValidPosition()
    {
        Vector3 snappedPosition;
        int attempts = 0;
        
        do
        {
            var cellMin = tilemap.cellBounds.min;
            var worldMin = tilemap.CellToWorld(cellMin);
            
            var randX = UnityEngine.Random.Range(0, mapSize.x);
            var randY = UnityEngine.Random.Range(0, mapSize.y);
            
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