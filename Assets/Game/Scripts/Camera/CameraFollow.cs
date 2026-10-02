using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private BoxCollider2D cameraBounds;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        // Keep following even when a scene has not assigned arena bounds yet.
        float blend = 1f - Mathf.Exp(-Mathf.Max(0f, smoothSpeed) * Time.deltaTime);
        if (cameraBounds == null)
        {
            Vector3 position = target.position;
            position.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, position, blend);
            return;
        }

        Bounds bounds = cameraBounds.bounds;

        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        float minX = bounds.min.x + cameraHalfWidth;
        float maxX = bounds.max.x - cameraHalfWidth;

        float minY = bounds.min.y + cameraHalfHeight;
        float maxY = bounds.max.y - cameraHalfHeight;

        float targetX;
        float targetY;

        if (minX > maxX)
            targetX = bounds.center.x;
        else
            targetX = Mathf.Clamp(target.position.x, minX, maxX);

        if (minY > maxY)
            targetY = bounds.center.y;
        else
            targetY = Mathf.Clamp(target.position.y, minY, maxY);

        Vector3 targetPosition = new Vector3(
            targetX,
            targetY,
            transform.position.z
        );

        Vector3 newPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            blend
        );

        // Pastikan hasil smoothing juga tidak keluar dari batas arena.
        if (minX <= maxX)
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        else
            newPosition.x = bounds.center.x;

        if (minY <= maxY)
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        else
            newPosition.y = bounds.center.y;

        transform.position = newPosition;
    }
}
