using UnityEngine;

public class FloatingGunAim : MonoBehaviour
{
    [SerializeField] private GameObject gunObject; // The visual gun object
    [SerializeField] private float orbitRadius = 1f; // Default radius if no sprite is provided
    [SerializeField] private SpriteRenderer orbitPathSprite; // The circular sprite that defines the orbit path
    
    [SerializeField] private Camera _mainCamera;
    private Vector3 _mousePosition;
    public Vector2 AimDirection { get; private set; }
    public Transform GunTransform => gunObject.transform;
    
    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
            
        if (orbitPathSprite != null)
        {
            // Calculate orbit radius based on the orbit path sprite bounds
            Bounds bounds = orbitPathSprite.bounds;
            // For a circular sprite, use the average of width and height
            orbitRadius = (bounds.extents.x + bounds.extents.y) / 2f;
            Debug.Log($"Orbit radius calculated from sprite: {orbitRadius} units");
        }
    }
    
    void Update()
    {
        // Get mouse position in world coordinates
        _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0f;
        
        // Calculate direction from player to mouse
        Vector2 directionToMouse = (_mousePosition - transform.position).normalized;
        
        // Calculate the closest point on the circle edge to the mouse direction
        if (orbitPathSprite != null)
        {
            // Use the sprite bounds to determine the edge point
            Bounds bounds = orbitPathSprite.bounds;
            
            // Get the center of the sprite
            Vector2 spriteCenter = bounds.center;
            
            // For a perfect circle, we can use the direction to determine the point on the edge
            // For an ellipse or irregular shape, this is an approximation
            float radius = (bounds.extents.x + bounds.extents.y) / 2f;
            
            // Position the gun on the edge of the circle in the direction of the mouse
            Vector2 edgePosition = spriteCenter + directionToMouse * radius;
            
            // Set the gun position and update the aim direction
            gunObject.transform.position = new Vector3(edgePosition.x, edgePosition.y, 0);
            AimDirection = directionToMouse;
        }
        else
        {
            // Fallback to using the manually set radius
            gunObject.transform.position = transform.position + (Vector3)directionToMouse * orbitRadius;
            AimDirection = directionToMouse;
        }
        
        // Rotate gun to face outward (away from center)
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        gunObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    // Call this to recalculate the orbit radius if the sprite changes
    public void RefreshOrbitRadius()
    {
        if (orbitPathSprite != null)
        {
            Bounds bounds = orbitPathSprite.bounds;
            orbitRadius = (bounds.extents.x + bounds.extents.y) / 2f;
        }
    }
}