using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public Vector2 dir = Vector2.zero;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.velocity = speed * dir;
    }
}