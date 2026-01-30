using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : Card
{
    [SerializeField] private TextMeshProUGUI title, damage, range, speed, damageType, aoeRange, price;
    [SerializeField] private Image icon;

    private const string damageText = cBeginTag + "Damage: " + cEndTag;
    private const string priceText = cBeginTag + "Price: " + cEndTag;
    private const string rangeText = cBeginTag + "Range: " + cEndTag;
    private const string speedText = cBeginTag + "Attack Speed: " + cEndTag;
    private const string damageTypeText = cBeginTag + "Damage Type: " + cEndTag;
    private const string aoeRangeText = cBeginTag + "AOE Range: " + cEndTag;

    public void Display(Tower tower, Sprite sprite)
    {
        icon.sprite = sprite;
        base.Display(tower);

        title.text = tower.biome == Biome.Plains
            ? tower.towerType.ToString()
            : (tower.biome + " " + tower.towerType);

        damage.text = damageText + tower.damage;
        range.text = rangeText + tower.range;
        speed.text = speedText + tower.speed;
        damageType.text = damageTypeText + (tower.damageType ? "Physical" : "Magical");
        aoeRange.text = aoeRangeText + tower.aoeRange;

        // NEW: show scaled price (falls back to base if scaler not present)
        int shownCost = GetScaledCost(tower);
        int placedCount = GetPlacedCount(tower);

        // If you DON'T want the "(x placed)" part, just set:
        // price.text = priceText + shownCost;
        price.text = priceText + shownCost;
    }

    private static int GetScaledCost(Tower tower)
    {
        if (tower == null) return 0;
        if (TowerPriceScaler.Instance == null) return tower.price;
        return TowerPriceScaler.Instance.GetCurrentPrice(tower);
    }

    private static int GetPlacedCount(Tower tower)
    {
        if (tower == null) return 0;
        if (TowerPriceScaler.Instance == null) return 0;
        return TowerPriceScaler.Instance.GetPlacedCount(tower);
    }
}
