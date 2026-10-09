using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Sound")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField, Range(0f, 1f)] private float walkVolume = 0.3f;
    [Tooltip("One step per 0.375-second walk animation cycle at the default speed.")]
    [SerializeField, Min(0.05f)] private float stepInterval = 0.375f;
    private float stepTimer;
    private AudioSource walkSource;
    private PlayerHealth playerHealth;
    private Vector2 previousPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        previousPosition = rb.position;
        walkSource = gameObject.AddComponent<AudioSource>();
        walkSource.playOnAwake = false;
        walkSource.spatialBlend = 0f;
        walkSource.volume = walkVolume;
        walkSource.clip = walkSound;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;
        if (Time.timeScale <= 0f || (playerHealth != null && playerHealth.IsDead))
        {
            movement = Vector2.zero;
            walkSource.Stop();
            stepTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        // Actual displacement prevents footsteps while pushing into a wall.
        bool moved = (rb.position - previousPosition).sqrMagnitude > 0.000001f;
        previousPosition = rb.position;
        bool walking = moved && movement.sqrMagnitude > 0.01f
            && (playerHealth == null || !playerHealth.IsDead);
        if (walking && walkSound != null)
        {
            stepTimer -= Time.fixedDeltaTime;
            if (stepTimer <= 0f)
            {
                walkSource.Play();
                stepTimer = Mathf.Max(0.05f, stepInterval);
            }
        }
        else
        {
            walkSource.Stop();
            stepTimer = 0f;
        }
        rb.linearVelocity = movement * moveSpeed;
    }

    private void OnDisable()
    {
        if (walkSource != null) walkSource.Stop();
    }
}
