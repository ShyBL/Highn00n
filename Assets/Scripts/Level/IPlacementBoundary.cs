using UnityEngine;

public interface IPlacementBoundary
{
    bool IsPointWithinBoundary(Vector3 worldPosition);
    Vector3 GetRandomPointWithinBoundary(Vector3 mapOrigin, Vector2 mapSize);
}