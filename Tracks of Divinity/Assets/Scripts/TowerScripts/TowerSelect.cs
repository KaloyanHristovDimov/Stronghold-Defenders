using UnityEngine;

public class TowerSelect : MonoBehaviour
{

    public int selectedTower = 0;
    public static TowerSelect Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSelectedTower(int towerNumber)
    {
        selectedTower = towerNumber;
    }
}
