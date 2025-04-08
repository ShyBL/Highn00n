using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [HideInInspector] public Vector2 dir = Vector2.zero;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        dir = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
            dir.x = -1;
        else if (Input.GetKey(KeyCode.D))
            dir.x = 1;
        if (Input.GetKey(KeyCode.W))
            dir.y = 1;
        else if (Input.GetKey(KeyCode.S))
            dir.y = -1;

        dir.Normalize();
        _rb.velocity = speed * dir;
    }
}