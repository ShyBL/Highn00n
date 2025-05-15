using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ColliderBoundary : IPlacementBoundary
{
    [SerializeField] private Collider2D boundaryCollider;
    
    public Collider2D BoundaryCollider => boundaryCollider;

    public bool IsPointWithinBoundary(Vector3 worldPosition)
    {
        if (boundaryCollider == null)
            return true; // If no collider set, allow anywhere
            
        Vector2 position2D = new Vector2(worldPosition.x, worldPosition.y);
        return boundaryCollider.OverlapPoint(position2D);
    }

    public Vector3 GetRandomPointWithinBoundary(Vector3 mapOrigin, Vector2 mapSize)
    {
        if (boundaryCollider == null)
        {
            // If no collider set, use the whole map
            float randX = UnityEngine.Random.Range(0, mapSize.x);
            float randY = UnityEngine.Random.Range(0, mapSize.y);
            return mapOrigin + new Vector3(randX, randY, 0);
        }
        
        // Get collider bounds
        Bounds bounds = boundaryCollider.bounds;
        
        // Try to find a point within the collider
        int maxAttempts = 50;
        for (int i = 0; i < maxAttempts; i++)
        {
            float randX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float randY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            Vector2 point = new Vector2(randX, randY);
            
            if (boundaryCollider.OverlapPoint(point))
            {
                return new Vector3(point.x, point.y, 0);
            }
        }
        
        // Fallback to center of bounds if random point finding fails
        return bounds.center;
    }
}