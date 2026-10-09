using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Weapon Settings")]
    [SerializeField] private float fireRate = 0.3f;

    private float nextFireTime;

    [Header("Sound")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField, Range(0f, 1f)] private float shotVolume = 0.65f;
    private AudioSource shotSource;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        shotSource = gameObject.AddComponent<AudioSource>();
        shotSource.playOnAwake = false;
        shotSource.spatialBlend = 0f;
        shotSource.volume = shotVolume;
        shotSource.clip = shotSound;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || (playerHealth != null && playerHealth.IsDead))
        {
            shotSource.Stop();
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bulletObject = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        bullet.Initialize(firePoint.right);
        // Restart the attack with each actual bullet, without stacking long tails.
        if (shotSound != null) shotSource.Play();
    }

    private void OnDisable()
    {
        if (shotSource != null) shotSource.Stop();
    }
}
