using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform weaponPivot;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Vector2 direction =
            mouseWorldPosition - weaponPivot.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        weaponPivot.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}