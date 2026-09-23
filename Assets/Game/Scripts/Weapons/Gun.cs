using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Weapon Settings")]
    [SerializeField] private float fireRate = 0.3f;

    private float nextFireTime;

    private void Update()
    {
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
    }
}