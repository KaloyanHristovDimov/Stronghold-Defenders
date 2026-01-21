using UnityEngine;

public class FixedPovCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float scrollSpeed = 250f;

    [SerializeField] private float minDistance = 25f;
    [SerializeField] private float maxDistance = 85f;
    [SerializeField] private float startDistance = 45f;

    [SerializeField] private bool constrainMoveToHorizontalPlane = true;

    private Quaternion lockedRot;
    private Vector3 pivot;
    private float distance;

    private void Awake()
    {
        lockedRot = transform.rotation;

        distance = Mathf.Clamp(startDistance, minDistance, maxDistance);

        Vector3 axis = (lockedRot * Vector3.forward).normalized;

        pivot = transform.position;
        transform.position = pivot + axis * distance;
    }

    private void LateUpdate()
    {
        transform.rotation = lockedRot;

        float dt = Time.deltaTime;

        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        Vector3 right = lockedRot * Vector3.right;
        Vector3 forward = lockedRot * Vector3.forward;

        if (constrainMoveToHorizontalPlane)
        {
            right.y = 0f;
            forward.y = 0f;
            if (right.sqrMagnitude > 0.0001f) right.Normalize();
            if (forward.sqrMagnitude > 0.0001f) forward.Normalize();
        }
        else
        {
            right.Normalize();
            forward.Normalize();
        }

        Vector3 move = right * x + forward * z;
        if (move.sqrMagnitude > 1f) move.Normalize();

        pivot += move * moveSpeed * dt;

        float scroll = Input.mouseScrollDelta.y;

        distance += scroll * scrollSpeed * dt;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 axis = (lockedRot * Vector3.forward).normalized;
        transform.position = pivot + axis * distance;
    }

    private void OnValidate()
    {
        if (maxDistance < minDistance)
            maxDistance = minDistance;

        startDistance = Mathf.Clamp(startDistance, minDistance, maxDistance);
    }
}
