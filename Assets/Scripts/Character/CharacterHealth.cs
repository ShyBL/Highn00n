using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    private const int MaxHealth = 100;
    private int _currentHealth;
    public UnityEvent onPlayerDeath;

    void Start()
    {
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log("Player took damage! Current health: " + _currentHealth);

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
}