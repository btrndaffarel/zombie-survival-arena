using UnityEngine;

public class BulletVisualRotation : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        RotateToDirection();
    }

    private void Update()
    {
        RotateToDirection();
    }

    private void RotateToDirection()
    {
        Vector2 direction = rb.linearVelocity;

        if (direction.sqrMagnitude < 0.01f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}