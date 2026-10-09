using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopDistance = 1.2f;

    private Rigidbody2D rb;
    private Transform player;

    [Header("Arrival Sound")]
    [SerializeField] private AudioClip arrivalSound;
    [SerializeField, Range(0f, 1f)] private float arrivalVolume = 0.45f;
    [SerializeField, Min(1f)] private float soundRange = 25f;
    private AudioSource arrivalSource;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        arrivalSource = gameObject.AddComponent<AudioSource>();
        arrivalSource.playOnAwake = false;
        arrivalSource.spatialBlend = 0f;
        arrivalSource.clip = arrivalSound;
        arrivalSource.volume = arrivalVolume;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
        UpdateSoundPosition();
        if (arrivalSound != null) arrivalSource.Play();
    }

    private void Update()
    {
        if (player == null || (playerHealth != null && playerHealth.IsDead))
        {
            arrivalSource.Stop();
            return;
        }
        if (Time.timeScale <= 0f) arrivalSource.Pause();
        else arrivalSource.UnPause();
        UpdateSoundPosition();
    }

    private void UpdateSoundPosition()
    {
        if (player == null) return;
        // Use player distance in the 2D plane, independent of camera Z offset.
        Vector2 offset = transform.position - player.position;
        float range = Mathf.Max(1f, soundRange);
        arrivalSource.volume = arrivalVolume * Mathf.Lerp(0.15f, 1f,
            1f - Mathf.Clamp01(offset.magnitude / range));
        arrivalSource.panStereo = Mathf.Clamp(offset.x / range, -0.75f, 0.75f);
    }

    private void OnDisable()
    {
        if (arrivalSource != null) arrivalSource.Stop();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer > stopDistance)
        {
            Vector2 direction =
                (player.position - transform.position).normalized;

            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
