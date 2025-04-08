using UnityEngine;
using System.Collections.Generic;

public class CharacterRangeAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public List<Transform> firePoints; // A list of fire points for each direction
    private CharacterAnimation characterAnimation;
    public float bulletSpeed = 10f;
    private void Start()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
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
        int direction = characterAnimation.GetFacingDirection();
        
        // Ensure direction is within bounds
        if (direction >= 0 && direction < firePoints.Count)
        {
            Transform selectedFirePoint = firePoints[direction]; // Get the fire point based on direction
            var thing = Instantiate(bulletPrefab, selectedFirePoint.position, selectedFirePoint.rotation);
            if (thing.TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = selectedFirePoint.right * bulletSpeed;
                //rb.AddForce(Vector2.up,ForceMode2D.Impulse);
            }
            
        }
        else
        {
            Debug.LogWarning("Invalid direction for firing!");
        }
    }
}