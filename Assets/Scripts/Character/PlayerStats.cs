using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Lloyd,
    Jack,
    Dollothy, 
    Mai,
    Antonio,
    Sally
}

[System.Serializable]
public class CharacterStats
{
    public int hp = 1;
    public int damage = 1;
    public float shotsPerSecond = 1f;
    public float movementSpeed = 5f;
    public bool hasDash = false;
    public bool hasAreaOfEffect = false;
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private CharacterType currentCharacter = CharacterType.Lloyd;
    
    private readonly Dictionary<CharacterType, CharacterStats> _statsDatabase = new()
    {
        { CharacterType.Lloyd, new CharacterStats 
            { 
                hp = 1, 
                damage = 2, 
                shotsPerSecond = 3f, 
                movementSpeed = 5f 
            } 
        },
        
        { CharacterType.Jack, new CharacterStats 
            { 
                hp = 1, 
                damage = 3, 
                shotsPerSecond = 2f, 
                movementSpeed = 3.5f, 
                hasDash = true 
            } 
        },
        
        { CharacterType.Dollothy, new CharacterStats 
            { 
                hp = 1, 
                damage = 6, 
                shotsPerSecond = 1f, 
                movementSpeed = 3.5f, 
                hasDash = true, 
                hasAreaOfEffect = true 
            } 
        },
        
        { CharacterType.Mai, new CharacterStats 
            { 
                hp = 1, 
                damage = 5, 
                shotsPerSecond = 2f, 
                movementSpeed = 6.5f 
            } 
        },
        
        { CharacterType.Antonio, new CharacterStats 
            { 
                hp = 1, 
                damage = 3, 
                shotsPerSecond = 2f, 
                movementSpeed = 3.5f, 
                hasDash = true, 
                hasAreaOfEffect = true 
            } 
        },
        
        { CharacterType.Sally, new CharacterStats 
            { 
                hp = 1, 
                damage = 1, 
                shotsPerSecond = 8f, 
                movementSpeed = 5f 
            } 
        }
    };

    public CharacterType CurrentCharacter
    {
        get { return currentCharacter; }
        set { 
            currentCharacter = value;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }

    public CharacterStats GetCurrentStats()
    {
        return GetStats(currentCharacter);
    }

    public CharacterStats GetStats(CharacterType character)
    {
        if (_statsDatabase.TryGetValue(character, out var stats))
            return stats;
        
        Debug.LogWarning($"Stats for character {character} not found! Returning default stats.");
        return new CharacterStats();
    }

    public void ChangeCharacter(CharacterType newCharacter)
    {
        if (!_statsDatabase.ContainsKey(newCharacter))
        {
            Debug.LogError($"Cannot change to character {newCharacter} - not found in database!");
        }
        else
        {
            CurrentCharacter = newCharacter;
            Debug.Log($"Changed character to: {newCharacter}");
        }
    }
}