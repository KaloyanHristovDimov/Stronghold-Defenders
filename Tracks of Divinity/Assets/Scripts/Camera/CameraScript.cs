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

    private float pitch;
    private float yaw;

    // Anchor point so that:
    // position = basePosition + forward * zoomOffset
    private Vector3 basePosition;

    // Offset along camera forward
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

        // ---------- ROTATION ----------
        float yawInput = 0f;

        if (allowRotation)
        {
            if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.E)) yawInput += 1f;
            if (invertRotationKeys) yawInput = -yawInput;

            yaw += yawInput * rotationSpeed * dt;
        }

        // Reset yaw to world 0 when pressing O
        if (Input.GetKeyDown(KeyCode.O))
        {
            yaw = 0f;
            yawInput = 1f; // force compensation
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 viewForward = rot * Vector3.forward;

        // Compensate anchor so camera doesn't move when yaw changes
        if (yawInput != 0f)
        {
            basePosition = worldPosBefore - viewForward * zoomOffset;
        }

        // ---------- MOVEMENT ----------
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        Vector3 right = rot * Vector3.right;
        Vector3 forward = rot * Vector3.forward;

        if (constrainMoveToHorizontalPlane)
        {
            right.y = 0f;
            forward.y = 0f;
            right.Normalize();
            forward.Normalize();
        }

        Vector3 move = right * x + forward * z;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        basePosition += move * moveSpeed * dt;

        // ---------- ZOOM ----------
        float scroll = Input.mouseScrollDelta.y;
        zoomOffset += scroll * scrollSpeed * dt;
        zoomOffset = Mathf.Clamp(zoomOffset, minDistance, maxDistance);

        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 forward = rot * Vector3.forward;

        transform.rotation = rot;
        transform.position = basePosition + forward * zoomOffset;
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
