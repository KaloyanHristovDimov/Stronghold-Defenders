using UnityEngine;

public class FixedPovCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private bool constrainMoveToHorizontalPlane = true;

    [Header("Zoom (along forward axis)")]
    [SerializeField] private float scrollSpeed = 250f;
    [Tooltip("How far BACK the camera can move from its start position (negative recommended)")]
    [SerializeField] private float minDistance = -40f;
    [Tooltip("How far FORWARD the camera can move from its start position")]
    [SerializeField] private float maxDistance = 40f;
    [Tooltip("Starting offset along the forward axis (0 = scene position)")]
    [SerializeField] private float startDistance = 0f;

    [Header("Rotation (Q / E)")]
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private bool invertRotationKeys = false;

    // Fixed pitch (your ~60°)
    private float pitch;
    private float yaw;

    // Immutable reference position
    private Vector3 basePosition;

    // Zoom offset along forward axis
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

        // --- Rotation (yaw only) ---
        if (allowRotation)
        {
            float yawInput = 0f;
            if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.E)) yawInput += 1f;

            if (invertRotationKeys)
                yawInput = -yawInput;

            yaw += yawInput * rotationSpeed * dt;
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // --- Movement (WASD moves base position) ---
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

        // --- Zoom ---
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
