using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Camera Bounds")]
    [Tooltip("Đánh dấu để giới hạn camera trong khung hình")]
    public bool useBounds = false;
    public Vector2 mapMinBounds = new Vector2(-10f, -6f);
    public Vector2 mapMaxBounds = new Vector2(10f, 6f);

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main; // Fallback
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        if (useBounds && cam != null)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.aspect * camHalfHeight;

            float minX = mapMinBounds.x + camHalfWidth;
            float maxX = mapMaxBounds.x - camHalfWidth;
            float minY = mapMinBounds.y + camHalfHeight;
            float maxY = mapMaxBounds.y - camHalfHeight;

            // Xử lý khi map quá nhỏ so với camera (tránh lỗi kẹt camera)
            if (minX > maxX) minX = maxX = (mapMinBounds.x + mapMaxBounds.x) / 2f;
            if (minY > maxY) minY = maxY = (mapMinBounds.y + mapMaxBounds.y) / 2f;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
