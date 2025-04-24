using UnityEngine;
using DG.Tweening;

public class MouseFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 0.5f;
    [SerializeField] private float followDelay = 0.2f;

    [Header("Bounds Settings")]
    [SerializeField] private Collider2D boundsCollider; // Assign in Inspector

    private bool isFollowing = false;
    private Camera mainCamera;
    private Vector2 minBounds, maxBounds;

    private void Awake()
    {
        mainCamera = Camera.main;
        SetBounds();
    }

    private void Update()
    {
        if (!isFollowing) return; // Only run when enabled

        FollowCursorSmoothly();
    }

    private void SetBounds()
    {
        if (boundsCollider != null)
        {
            Bounds b = boundsCollider.bounds;
            minBounds = b.min;
            maxBounds = b.max;
        }
    }

    private void FollowCursorSmoothly()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

        // Clamp position within collider bounds
        worldPos.x = Mathf.Clamp(worldPos.x, minBounds.x, maxBounds.x);
        worldPos.y = Mathf.Clamp(worldPos.y, minBounds.y, maxBounds.y);

        // Tween position to follow cursor smoothly within bounds
        transform.DOMove(new Vector3(worldPos.x, worldPos.y, transform.position.z), followSpeed).SetEase(Ease.OutExpo);
    }

    public void EnableFollower()
    {
        isFollowing = true;
    }

    public void DisableFollower()
    {
        isFollowing = false;
    }
}