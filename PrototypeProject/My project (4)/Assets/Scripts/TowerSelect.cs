using UnityEngine;

public class TowerSelect : MonoBehaviour
{
    public int selectedTower = 0; // 0 = none, 1 = basic tower, 2 = blast tower
    
    public void selectTower(int type)
    {
        selectedTower = type;
        TutorialManager.Instance.OnTowerSelected();
    }
}
