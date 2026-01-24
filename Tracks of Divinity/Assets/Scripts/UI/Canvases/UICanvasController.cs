using UnityEngine;

public class UICanvasController : MonoBehaviour
{
    [SerializeField] private GoldCounter goldCounter;
    [SerializeField] private CounterController waveCounter, healthCounter;

    public static UICanvasController inst;
    public static GoldCounter GoldCounter => inst.goldCounter;
    public static CounterController WaveCounter => inst.waveCounter;
    public static CounterController HealthCounter => inst.healthCounter;

    private void Awake() => inst = this;
}
