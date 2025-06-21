using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    [Header("Character Integration")]
    [SerializeField] private PlayerStats playerStats;
    
    [Header("Events")]
    public UnityEvent onPlayerDeath;
    
    private int _currentHealth;
    private int _maxHealth;

    void Start()
    {
        // Auto-find PlayerStats if not assigned
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        
        // Get character-specific health from PlayerStats
        if (playerStats != null)
        {
            _maxHealth = playerStats.GetCurrentStats().hp;
            Debug.Log($"Character health set to: {_maxHealth} (from {playerStats.CurrentCharacter})");
        }
        else
        {
            _maxHealth = 100; // Fallback health
            Debug.LogWarning("PlayerStats not found! Using fallback health: 100");
        }
        
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Current health: {_currentHealth}/{_maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        onPlayerDeath?.Invoke();
    }
    
    // Public getters for UI
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public float HealthPercentage => (float)_currentHealth / _maxHealth;
}