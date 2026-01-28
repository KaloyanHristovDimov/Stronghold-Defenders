using UnityEngine;

public class FixedPovCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private bool constrainMoveToHorizontalPlane = true;

    [Header("Zoom (offset along view forward)")]
    [SerializeField] private float scrollSpeed = 300f;
    [SerializeField] private float minDistance = 45f;
    [SerializeField] private float maxDistance = 90f;
    [SerializeField] private float startDistance = 60f;

    [Header("Rotation (Q / E)")]
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private bool invertRotationKeys = false;

    [Header("Mouse Look (Hold RMB)")]
    [SerializeField] private bool allowMouseLook = true;
    [SerializeField] private float mouseSensitivity = 360f;

    [Header("Mouse Pan (Hold MMB)")]
    [SerializeField] private bool allowMousePan = true;
    [SerializeField] private float mousePanSpeed = 150f;

    private float pitch;
    private float yaw;

    private Vector3 basePosition;
    private float zoomOffset;

    private void Awake()
    {
        Vector3 e = transform.eulerAngles;
        pitch = e.x;
        yaw = e.y;

        basePosition = transform.position;
        zoomOffset = Mathf.Clamp(startDistance, minDistance, maxDistance);

        UpdateTransform();
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        Vector3 worldPosBefore = transform.position;

        // O reset: set yaw to 0, BUT keep world position the same
        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector3 worldPos = transform.position;

            yaw = 0f;

            Quaternion rot0 = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 fwd0 = rot0 * Vector3.forward;

            basePosition = worldPos - fwd0 * zoomOffset;

            UpdateTransform();
            return;
        }

        // ---------- ROTATION ----------
        float yawDelta = 0f;

        if (allowRotation)
        {
            if (Input.GetKey(KeyCode.Q)) yawDelta -= rotationSpeed * dt;
            if (Input.GetKey(KeyCode.E)) yawDelta += rotationSpeed * dt;
            if (invertRotationKeys) yawDelta = -yawDelta;
        }

        if (allowMouseLook && Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            yawDelta += mouseX * mouseSensitivity * dt;
        }

        yaw += yawDelta;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 viewForward = rot * Vector3.forward;

        // Keep position stable while yaw changes (Q/E or RMB)
        if (yawDelta != 0f)
            basePosition = worldPosBefore - viewForward * zoomOffset;

        // ---------- MOVEMENT (WASD) ----------
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        Vector3 rightMove = rot * Vector3.right;
        Vector3 forwardMove = rot * Vector3.forward;

        if (constrainMoveToHorizontalPlane)
        {
            rightMove.y = 0f;
            forwardMove.y = 0f;
            rightMove.Normalize();
            forwardMove.Normalize();
        }

        Vector3 move = rightMove * x + forwardMove * z;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        basePosition += move * moveSpeed * dt;

        // ---------- MOUSE PAN (MMB drag — inverted) ----------
        if (allowMousePan && Input.GetMouseButton(2))
        {
            float mx = Input.GetAxisRaw("Mouse X");
            float my = Input.GetAxisRaw("Mouse Y");

            Vector3 rightPan = rot * Vector3.right;
            Vector3 forwardPan = rot * Vector3.forward;

            if (constrainMoveToHorizontalPlane)
            {
                rightPan.y = 0f;
                forwardPan.y = 0f;
                rightPan.Normalize();
                forwardPan.Normalize();
            }

            Vector3 pan = -rightPan * mx + -forwardPan * my;
            basePosition += pan * mousePanSpeed * dt;
        }

        // ---------- ZOOM ----------
        float scroll = Input.mouseScrollDelta.y;
        zoomOffset += scroll * scrollSpeed * dt;
        zoomOffset = Mathf.Clamp(zoomOffset, minDistance, maxDistance);

        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = rot;
        transform.position = basePosition + (rot * Vector3.forward) * zoomOffset;
    }

    private void OnValidate()
    {
        if (maxDistance < minDistance)
            maxDistance = minDistance;

        startDistance = Mathf.Clamp(startDistance, minDistance, maxDistance);
    }

    public void AddWorldOffset(Vector3 offset)
    {
        basePosition += offset;
        transform.position += offset;
    }
}
