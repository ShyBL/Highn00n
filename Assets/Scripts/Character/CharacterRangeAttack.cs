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
        // Determine last movement direction
        var lastX = _characterAnimation.GetLastInputX();
        var lastY = _characterAnimation.GetLastInputY();

        // Select the appropriate fire point using a switch expression
        var selectedFirePoint = lastY switch
        {
            > 0 => firePoints[1], // Up
            < 0 => firePoints[0], // Down
            _ => lastX switch
            {
                > 0 => firePoints[2], // Right
                < 0 => firePoints[3], // Left
                _ => firePoints[0] // Default to Down if no direction is set
            }
        };

        if (selectedFirePoint == null)
        {
            Debug.LogWarning("No valid fire point found!");
            return;
        }

        // Instantiate bullet and apply velocity
        var bullet = Instantiate(bulletPrefab, selectedFirePoint.position, selectedFirePoint.rotation);
        
        if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = selectedFirePoint.up * bulletSpeed; // Fire in the correct direction
        }
        else
        {
            Debug.LogError("Bullet object missing Rigidbody2D component!");
        }
    }
}