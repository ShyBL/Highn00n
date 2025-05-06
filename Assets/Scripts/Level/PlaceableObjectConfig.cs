using System;
using UnityEngine;

[Serializable]
public class PlaceableObjectConfig
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private PlacementQuantityType quantityType;
    [SerializeField] private int fixedQuantity = 1;

    public GameObject Prefab => prefab;
    
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