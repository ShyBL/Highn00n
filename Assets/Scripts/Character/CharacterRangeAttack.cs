using UnityEngine;
using System.Collections.Generic;

public class CharacterRangeAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private List<Transform> firePoints; // A list of fire points for each direction
    [SerializeField] private float bulletSpeed = 10f;
    
    private CharacterAnimation _characterAnimation;
    private void Start()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        int direction = _characterAnimation.GetFacingDirection();
        
        // Ensure direction is within bounds
        if (direction >= 0 && direction < firePoints.Count)
        {
            Transform selectedFirePoint = firePoints[direction]; // Get the fire point based on direction
            var thing = Instantiate(bulletPrefab, selectedFirePoint.position, selectedFirePoint.rotation);
            if (thing.TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = selectedFirePoint.right * bulletSpeed;
            }
            
        }
        else
        {
            Debug.LogWarning("Invalid direction for firing!");
        }
    }
}