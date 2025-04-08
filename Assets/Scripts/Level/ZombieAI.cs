using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 1;
    public int maxHealth = 6;
    private int currentHealth;

    private Transform player;
    private Rigidbody2D rb;
    private float attackTimer;
    
    public GameObject graveMarker; // Grave GameObject
    public GameObject artGameObject;
    public float resurrectionTime = 12f;
    // Time before resurrection
    public UnityEvent onZombieDeath;
    public UnityEvent onZombieResurrect;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        graveMarker.SetActive(false); // Grave starts disabled
        artGameObject.SetActive(true);
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    void AttackPlayer()
    {
        if (attackTimer <= 0)
        {
            player.GetComponent<CharacterHealth>()?.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            var dmg = bullet.damage;
            
            Destroy(bullet.gameObject);
            
            TakeDamage(dmg);
        }
    }

    public void TakeDamage(int amount)
    {
        Vector2 knockbackDir = (transform.position - player.position).normalized;
        rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        onZombieDeath?.Invoke();
        StartCoroutine(Resurrect());
    }

    IEnumerator Resurrect()
    {
        yield return new WaitForSeconds(resurrectionTime);
        currentHealth = maxHealth; // Reset health
        onZombieResurrect?.Invoke();
    }
}
