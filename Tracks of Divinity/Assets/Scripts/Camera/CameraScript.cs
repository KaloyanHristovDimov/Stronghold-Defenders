using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float scrollSpeed = 25f;

    [Header("Options")]
    [Tooltip("If true, WASD movement is constrained to a horizontal plane (Y stays constant).")]
    [SerializeField] private bool constrainWASDToHorizontalPlane = true;

    [Tooltip("If true, scroll dolly is constrained to a horizontal plane (Y stays constant).")]
    [SerializeField] private bool constrainScrollToHorizontalPlane = false;

    [Tooltip("If true, movement speed scales with Shift.")]
    [SerializeField] private bool enableShiftBoost = true;

    [SerializeField] private float shiftMultiplier = 2f;

    private Quaternion lockedRotation;

    private void Awake()
    {
        // Lock whatever rotation the camera currently has.
        lockedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Enforce locked rotation every frame.
        transform.rotation = lockedRotation;

        float dt = Time.deltaTime;

        float boost = 1f;
        if (enableShiftBoost && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            boost = shiftMultiplier;

        // WASD input
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        // Build movement in camera-relative space.
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        if (constrainWASDToHorizontalPlane)
        {
            right.y = 0f;
            forward.y = 0f;
            right = right.sqrMagnitude > 0f ? right.normalized : Vector3.right;
            forward = forward.sqrMagnitude > 0f ? forward.normalized : Vector3.forward;
        }

        Vector3 wasdMove = (right * x + forward * z);
        if (wasdMove.sqrMagnitude > 1f) wasdMove.Normalize();

        // Scroll dolly (projected axis = camera forward)
        float scroll = Input.mouseScrollDelta.y; // positive = scroll up by default
        Vector3 dollyDir = transform.forward;

        if (constrainScrollToHorizontalPlane)
        {
            dollyDir.y = 0f;
            dollyDir = dollyDir.sqrMagnitude > 0f ? dollyDir.normalized : Vector3.forward;
        }

        Vector3 dollyMove = dollyDir * (scroll * scrollSpeed);

        // Apply combined movement
        transform.position += (wasdMove * moveSpeed * boost + dollyMove) * dt;
    }
}
