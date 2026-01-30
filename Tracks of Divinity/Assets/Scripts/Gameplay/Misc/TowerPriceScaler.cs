using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks how many times each Tower ScriptableObject has been placed,
/// and calculates an inflated price based on that count.
/// </summary>
public sealed class TowerPriceScaler : MonoBehaviour
{
    public static TowerPriceScaler Instance { get; private set; }

    [Header("Price Scaling")]
    [Tooltip("Example: 0.15 = +15% each time you place the same tower (per Tower ScriptableObject).")]
    [SerializeField, Range(0f, 2f)] private float priceIncreasePercent = 0.15f;

    // Keyed by the Tower ScriptableObject asset (Desert Archer, Snow Archer, etc.)
    private readonly Dictionary<Tower, int> _placedCount = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetPlacedCount(Tower towerData)
    {
        if (towerData == null) return 0;
        return _placedCount.TryGetValue(towerData, out int c) ? c : 0;
    }

    public int GetCurrentPrice(Tower towerData)
    {
        if (towerData == null) return 0;

        int basePrice = towerData.price;
        int alreadyPlaced = GetPlacedCount(towerData);

        float multiplier = Mathf.Pow(1f + priceIncreasePercent, alreadyPlaced);
        return Mathf.CeilToInt(basePrice * multiplier);
    }

    public void RegisterPlaced(Tower towerData)
    {
        if (towerData == null) return;

        if (_placedCount.TryGetValue(towerData, out int c))
            _placedCount[towerData] = c + 1;
        else
            _placedCount.Add(towerData, 1);
    }

    public void ResetAll()
    {
        _placedCount.Clear();
    }
}
