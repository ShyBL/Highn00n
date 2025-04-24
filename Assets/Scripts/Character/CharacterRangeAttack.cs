using UnityEngine;
using System.Collections.Generic;

public class CharacterRangeAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    
    private CharacterAnimation _characterAnimation;
    private FloatingGunAim _floatingGunAim;
    
    private void Start()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
        _floatingGunAim = GetComponent<FloatingGunAim>();
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
        var gunTransform = _floatingGunAim.GunTransform;
        var direction = _floatingGunAim.AimDirection;
        
        var bullet = Instantiate(bulletPrefab, gunTransform.position, gunTransform.rotation);
        if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}