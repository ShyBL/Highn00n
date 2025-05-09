using System;
using UnityEngine;

[Serializable]
public class PlaceableObjectConfig
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private PlacementQuantityType quantityType;
    [SerializeField] private int fixedQuantity = 1;
    [SerializeField] private PlacementZone placementZone = PlacementZone.Anywhere;

    public GameObject Prefab => prefab;
    public PlacementZone Zone => placementZone;

    
    public int GetQuantity(int level, LevelScalingConfig scalingConfig)
    {
        return quantityType switch
        {
            PlacementQuantityType.Fixed => fixedQuantity,
            PlacementQuantityType.PowerUpCrates => scalingConfig.GetPowerUpCrateCount(level),
            PlacementQuantityType.ClockTowers => scalingConfig.GetClockTowerCount(level),
            _ => 1
        };
    }
}