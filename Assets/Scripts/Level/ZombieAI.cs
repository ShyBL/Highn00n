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
    private int _currentHealth;

    private Transform _player;
    private Rigidbody2D _rb;
    private float _attackTimer;
    
    public GameObject graveMarker;
    public GameObject artGameObject;
    public float resurrectionTime = 12f;
    
    public UnityEvent onZombieDeath;
    public UnityEvent onZombieResurrect;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = maxHealth;
        graveMarker.SetActive(false); // Grave starts disabled
        artGameObject.SetActive(true);
    }

    void Update()
    {
        if (_player == null || _currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

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
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.velocity = direction * moveSpeed;
    }

    void AttackPlayer()
    {
        if (_attackTimer <= 0)
        {
            _player.GetComponent<CharacterHealth>()?.TakeDamage(damage);
            _attackTimer = attackCooldown;
        }
        else
        {
            _attackTimer -= Time.deltaTime;
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
        Vector2 knockbackDir = (transform.position - _player.position).normalized;
        _rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);

        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onZombieDeath?.Invoke();
        StartCoroutine(Resurrect());
    }

    private IEnumerator Resurrect()
    {
        yield return new WaitForSeconds(resurrectionTime);
        _currentHealth = maxHealth; // Reset health
        onZombieResurrect?.Invoke();
    }
}
