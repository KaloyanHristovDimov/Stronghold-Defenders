using UnityEngine;

public class TileSpinner : MonoBehaviour
{
    private static readonly float spinSpeed = 45f;

    private void Update() => transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
}
