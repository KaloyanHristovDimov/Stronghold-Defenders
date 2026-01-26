using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : Card
{
    [SerializeField] private TextMeshProUGUI title, biome, damage, range, speed, aoeRange, price;
    [SerializeField] private Image icon;

    public override void Display()
    {
        base.Display();
    }
}
