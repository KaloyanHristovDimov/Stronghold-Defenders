using UnityEngine;

public class UICanvasController : MonoBehaviour
{
    [SerializeField] private GoldCounter goldCounter;
    [SerializeField] private CounterController waveCounter, healthCounter;
    [SerializeField] private LoseWindow loseScreen;

    public static UICanvasController inst;
    public static GoldCounter GoldCounter => inst.goldCounter;
    public static CounterController WaveCounter => inst.waveCounter;
    public static CounterController HealthCounter => inst.healthCounter;
    public static LoseWindow LoseScreen => inst.loseScreen;

    private void Awake()
    {
        inst = this;
        Persister.SetCursor();
    }
}
