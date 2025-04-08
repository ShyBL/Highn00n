using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public UnityEvent onPlayerDeath; // Event triggered on death

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        onPlayerDeath?.Invoke(); // Trigger death event (for respawn, game over, etc.)
        //gameObject.SetActive(false); // Disable player (adjust as needed)
    }
}