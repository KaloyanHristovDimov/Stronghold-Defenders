using System.Collections.Generic;
using UnityEngine;

public class UICanvasController : MonoBehaviour
{
    [SerializeField] private GoldCounter goldCounter;
    [SerializeField] private CounterController waveCounter, healthCounter;
    [SerializeField] private TowerCard mainTowerCard;
    [SerializeField] private LoseWindow loseScreen;
    [SerializeField] private ChoiceWindow choiceWindow;
    [SerializeField] private List<BiomeTowerPair> towers;

    public static UICanvasController inst;
    public static GoldCounter GoldCounter => inst.goldCounter;
    public static CounterController WaveCounter => inst.waveCounter;
    public static CounterController HealthCounter => inst.healthCounter;
    public static TowerCard MainTowerCard => inst.mainTowerCard;
    public static LoseWindow LoseScreen => inst.loseScreen;
    public static ChoiceWindow ChoiceWindow => inst.choiceWindow;
    public static List<BiomeTowerPair> Towers => inst.towers;

    public static RectTransform canvasRectT, towerCardRectT;
    public static TowerButton currentTowerButton;
    public static TowerSpawn currentTowerSpawnPoint;
    public static List<GameObject> TowerSpawnpoints;

    private void Awake()
    {
        inst = this;
        currentTowerSpawnPoint = null;
        TowerSpawnpoints = new();
        canvasRectT = GetComponent<RectTransform>();
        towerCardRectT = mainTowerCard.GetComponent<RectTransform>();
        Persister.SetCursor();
    }

    public static void ActivateTowerSpawnPoints(bool activate = false) => TowerSpawnpoints.ForEach(spawnpoint => spawnpoint.SetActive(activate));

    private void Update()
    {
        if(!towerCardRectT.gameObject.activeSelf)
            return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectT, Input.mousePosition, null, out var mousePos);
        towerCardRectT.anchoredPosition = new(mousePos.x, mousePos.y);
    }
}
