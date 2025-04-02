using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Vector2 moveDirection;
    private Vector2 aimDirection;

    void Update()
    {
        // Movement input
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");

        // Aim input
        aimDirection.x = Input.GetAxis("AimHorizontal");
        aimDirection.y = Input.GetAxis("AimVertical");

        // Shooting
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // Move the player
        transform.Translate(moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Rotate to aim direction
        if (aimDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}