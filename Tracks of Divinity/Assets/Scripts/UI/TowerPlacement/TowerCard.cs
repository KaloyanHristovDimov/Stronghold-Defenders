using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : Card
{
    [SerializeField] private TextMeshProUGUI title, damage, range, speed, damageType, aoeRange, price;
    [SerializeField] private Image icon;

    private const string damageText = cBeginTag + "Damage: " + cEndTag, priceText = cBeginTag + "Price: " + cEndTag,
        rangeText = cBeginTag + "Range: " + cEndTag, speedText = cBeginTag + "Attack Speed: " + cEndTag,
        damageTypeText = cBeginTag + "Damage Type: " + cEndTag, aoeRangeText = cBeginTag + "AOE Range: " + cEndTag;

    public void Display(Tower tower)
    {
        base.Display(tower);
        title.text = tower.biome == Biome.Plains ? tower.towerType.ToString() : (tower.biome.ToString() + " " + tower.towerType.ToString());
        damage.text = damageText + tower.damage;
        range.text = rangeText + tower.range;
        speed.text = speedText + tower.speed;
        damageType.text = damageTypeText + (tower.damageType ? "Physical" : "Magical");
        aoeRange.text = aoeRangeText + tower.aoeRange;
        price.text = priceText + tower.price;
    }
}
