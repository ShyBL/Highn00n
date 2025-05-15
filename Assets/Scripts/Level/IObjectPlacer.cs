using UnityEngine;

public interface IObjectPlacer
{
    void PlaceObject(GameObject prefab, IPlacementBoundary boundary, Transform parent);
}