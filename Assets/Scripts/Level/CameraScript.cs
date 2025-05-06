using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraSpeed = 2f;
    private Vector3 _offset = new(1f, 0.5f, -5f);

    private void Start() 
    {
        transform.position = player.position + _offset;
    }

    // Follow player
    void Update () 
    {
        Vector3 desiredPosition = player.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, cameraSpeed*2 * Time.deltaTime);
        transform.position = smoothedPosition;
        CheckPlayerDir();
    }
    
    void CheckPlayerDir() 
    {
        Vector3 targetOffset;
        if (Mathf.Approximately(player.transform.localScale.x, 1))
            targetOffset = new Vector3 (1f, 0.5f, -5f);
        else
            targetOffset = new Vector3 (-1f, 0.5f, -5f);
        _offset = Vector3.Lerp(_offset, targetOffset, cameraSpeed * Time.deltaTime);
    }
}