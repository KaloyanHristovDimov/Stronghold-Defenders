using UnityEngine;

public class TileSpinner : MonoBehaviour
{
    private float spinSpeed = 10f;

    private void Start() => InvokeRepeating(nameof(FlipDirection), .5f, 1f);

    private void FlipDirection() => spinSpeed *= -1f;

    private void Update() => transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
}
