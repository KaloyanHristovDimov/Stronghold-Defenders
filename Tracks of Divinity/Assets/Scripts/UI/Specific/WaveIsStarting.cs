using TMPro;
using UnityEngine;

public class WaveIsStarting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveStartingText;

    private const float fadeSpeedOut = .25f;

    public void Activate()
    {
        Color color = waveStartingText.color;
        color.a = 1f;
        waveStartingText.color = color;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Color color = waveStartingText.color;
        color.a -= fadeSpeedOut * Time.deltaTime;
        waveStartingText.color = color;

        if(color.a <= 0) gameObject.SetActive(false);
    }
}