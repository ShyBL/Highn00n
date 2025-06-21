using UnityEngine;

[CreateAssetMenu(fileName = "LevelScalingConfig", menuName = "Game/Level Scaling Configuration")]
public class LevelScalingConfig : ScriptableObject
{
    [Header("Power-Up Crates Scaling")]
    [SerializeField] private int basePowerUpCrates = 2;
    [SerializeField] private int powerUpCratesIncrement = 2;
    [SerializeField] private int powerUpCratesLevelInterval = 10;

    [Header("Clock Towers Scaling")]
    [SerializeField] private int baseClockTowers = 1;
    [SerializeField] private int clockTowersIncrement = 1;
    [SerializeField] private int clockTowersLevelInterval = 5;

    [Header("Zombie Spawning Scaling")]
    [SerializeField] private int baseZombies = 3;
    [SerializeField] private int zombieIncrement = 2;
    [SerializeField] private int zombieLevelInterval = 5;

    public int GetPowerUpCrateCount(int level)
    {
        if (level <= powerUpCratesLevelInterval)
            return basePowerUpCrates;
        
        return basePowerUpCrates + ((level - 1) / powerUpCratesLevelInterval) * powerUpCratesIncrement;
    }

    public int GetClockTowerCount(int level)
    {
        return baseClockTowers + (level / clockTowersLevelInterval);
    }

    public int GetZombieCount(int level)
    {
        return baseZombies + (level / zombieLevelInterval) * zombieIncrement;
    }
}