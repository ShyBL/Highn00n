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

    [Header("Movement Delay Settings")]
    [Range(0.2f, 5f)]
    public float reactionTime = 1f;  // Higher = slower reaction
    
    // Curve will be randomly generated at start
    private AnimationCurve _directionChangeCurve;
    
    private Transform _player;
    private Rigidbody2D _rb;
    private float _attackTimer;
    private Vector2 _currentMoveDirection;
    private Vector2 _targetMoveDirection;
    private float _directionChangeTimer = 0f;
    
    public GameObject graveMarker;
    public GameObject artGameObject;
    public float resurrectionTime = 12f;
    
    public UnityEvent onZombieDeath;
    public UnityEvent onZombieResurrect;
    
    private Animator _animator;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _currentHealth = maxHealth;
        graveMarker.SetActive(false); // Grave starts disabled
        artGameObject.SetActive(true);
        
        // Generate a random animation curve for this zombie's personality
        GenerateRandomMovementCurve();
        
        // Initialize movement direction
        if (_player != null)
        {
            _currentMoveDirection = (_player.position - transform.position).normalized;
            _targetMoveDirection = _currentMoveDirection;
        }
    }

    // Create a random animation curve to give this zombie a unique behavior
    void GenerateRandomMovementCurve()
    {
        // Choose from several curve types
        int curveType = Random.Range(0, 5);
        
        _directionChangeCurve = new AnimationCurve();
        
        switch(curveType)
        {
            case 0: // Very sluggish start, quick end
                _directionChangeCurve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
                _directionChangeCurve.AddKey(new Keyframe(0.6f, 0.1f, 0.1f, 0.5f));
                _directionChangeCurve.AddKey(new Keyframe(0.8f, 0.4f, 1.5f, 2.5f));
                _directionChangeCurve.AddKey(new Keyframe(1f, 1f, 3f, 0f));
                break;
                
            case 1: // Steady, linear-ish response
                _directionChangeCurve.AddKey(new Keyframe(0f, 0f, 0f, 0.5f));
                _directionChangeCurve.AddKey(new Keyframe(0.3f, 0.15f, 0.5f, 0.5f));
                _directionChangeCurve.AddKey(new Keyframe(0.7f, 0.5f, 0.7f, 1f));
                _directionChangeCurve.AddKey(new Keyframe(1f, 1f, 1f, 0f));
                break;
                
            case 2: // Quick start, plateaus in middle, quick end
                _directionChangeCurve.AddKey(new Keyframe(0f, 0f, 0f, 2f));
                _directionChangeCurve.AddKey(new Keyframe(0.3f, 0.4f, 1f, 0.1f));
                _directionChangeCurve.AddKey(new Keyframe(0.7f, 0.5f, 0.1f, 1f));
                _directionChangeCurve.AddKey(new Keyframe(1f, 1f, 2f, 0f));
                break;
                
            case 3: // Jerky movement - starts, stops, starts again
                _directionChangeCurve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
                _directionChangeCurve.AddKey(new Keyframe(0.2f, 0.3f, 2f, 0f));
                _directionChangeCurve.AddKey(new Keyframe(0.5f, 0.35f, 0f, 0f));
                _directionChangeCurve.AddKey(new Keyframe(0.7f, 0.4f, 0f, 1f));
                _directionChangeCurve.AddKey(new Keyframe(1f, 1f, 3f, 0f));
                break;
                
            case 4: // Very delayed reaction then sudden movement
                _directionChangeCurve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
                _directionChangeCurve.AddKey(new Keyframe(0.7f, 0.1f, 0.2f, 0.5f));
                _directionChangeCurve.AddKey(new Keyframe(0.9f, 0.6f, 3f, 3f));
                _directionChangeCurve.AddKey(new Keyframe(1f, 1f, 3f, 0f));
                break;
        }
        
        // Add some randomness to the curve
        for (int i = 0; i < _directionChangeCurve.keys.Length; i++)
        {
            Keyframe key = _directionChangeCurve.keys[i];
            
            // Don't modify start and end keys too much
            if (i > 0 && i < _directionChangeCurve.keys.Length - 1)
            {
                key.time = Mathf.Clamp01(key.time + Random.Range(-0.1f, 0.1f));
                key.value = Mathf.Clamp01(key.value + Random.Range(-0.1f, 0.1f));
            }
            
            _directionChangeCurve.MoveKey(i, key);
        }
    }

    void Update()
    {
        if (_player == null || _currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        // Always update the target direction to point toward the player
        _targetMoveDirection = (_player.position - transform.position).normalized;
        
        // Always update the current movement direction using the animation curve
        UpdateMovementDirection();

        // Decide what to do based on distance
        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Stop movement when in attack range
            _rb.velocity = Vector2.zero;
            AttackPlayer();
        }
    }
    
    void UpdateMovementDirection()
    {
        // Here's where we directly use reactionTime to control the delay
        // Higher reactionTime = slower increments to the timer = slower reaction
        _directionChangeTimer += Time.deltaTime / reactionTime;
        
        // Clamp to prevent potential issues
        _directionChangeTimer = Mathf.Clamp01(_directionChangeTimer);
        
        // Use the animation curve to control the interpolation rate
        float curveValue = _directionChangeCurve.Evaluate(_directionChangeTimer);
        
        // Interpolate the direction based on curve
        _currentMoveDirection = Vector2.Lerp(_currentMoveDirection, _targetMoveDirection, curveValue);
        
        // If we've reached the target direction, reset the timer
        if (_directionChangeTimer >= 1.0f || Vector2.Dot(_currentMoveDirection.normalized, _targetMoveDirection.normalized) > 0.99f)
        {
            _directionChangeTimer = 0f;
        }
    }

    void MoveTowardsPlayer()
    {
        // Move using the smoothed direction
        _rb.velocity = _currentMoveDirection * moveSpeed;
        
        // Update animator if available
        if (_animator != null)
        {
            _animator.SetFloat("Speed", _rb.velocity.magnitude);
        }
    }

    void AttackPlayer()
    {
        if (_attackTimer <= 0)
        {
            // Play attack animation if available
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }
            
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
        
        // Reset direction change time on hit to make zombies respond after getting hit
        _directionChangeTimer = 0f;
        
        // Play hit animation if available
        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Stop all movement
        _rb.velocity = Vector2.zero;
        
        // Play death animation if available
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        
        // Make the grave marker appear and hide the zombie model
        graveMarker.SetActive(true);
        artGameObject.SetActive(false);
        
        onZombieDeath?.Invoke();
        StartCoroutine(Resurrect());
    }

    private IEnumerator Resurrect()
    {
        yield return new WaitForSeconds(resurrectionTime);
        
        // Regenerate the movement curve when resurrecting to get new behavior
        GenerateRandomMovementCurve();
        
        // Play resurrection animation/effect
        if (_animator != null)
        {
            _animator.SetTrigger("Resurrect");
        }
        
        // Show zombie, hide grave
        artGameObject.SetActive(true);
        graveMarker.SetActive(false);
        
        _currentHealth = maxHealth; // Reset health
        onZombieResurrect?.Invoke();
    }
    
    // Visual debugging
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && _player != null)
        {
            // Draw current direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _currentMoveDirection * 1.5f);
            
            // Draw target direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _targetMoveDirection * 1.5f);
        }
    }
}