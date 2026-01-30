using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class TowerPricePair
{
    public Tower tower;
    public GameObject prefab => tower.prefab;
    public int price => tower.price;
}

[System.Serializable]
public class BiomeTowerPair
{
    public Biome biome;
    public List<TowerPricePair> pair;
}

public class TowerSpawn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool initialized;
    private int biome;

    public void Initialize(int tileBiome)
    {
        UICanvasController.TowerSpawnpoints.Add(gameObject);
        gameObject.SetActive(false);
        initialized = true;
        biome = tileBiome;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized) return;

        UICanvasController.currentTowerSpawnPoint = this;

        // Your current UI method:
        // NOTE: this probably shows base price, not scaled price.
        // We'll keep it working, and you can upgrade the UI later (see notes below).
        UICanvasController.currentTowerButton.ShowCard(biome, (int)UICanvasController.currentTowerButton.type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!initialized) return;

        UICanvasController.currentTowerSpawnPoint = null;
        UICanvasController.currentTowerButton.HideCard();
    }

    public void TrySpawnTower()
    {
        TowerPricePair pair = UICanvasController.Towers[biome].pair[(int)UICanvasController.currentTowerButton.type];
        OnPointerExit(null);

        if (pair == null || pair.tower == null)
            return;

        // --- NEW: scaled price ---
        int cost = GetScaledCost(pair.tower);

        if (!UICanvasController.GoldCounter.CanAfford(cost))
            return;

        UICanvasController.GoldCounter.DecrementCount(cost);

        Instantiate(pair.prefab, transform.position, Quaternion.identity);

        // --- NEW: register this placement so the next one costs more ---
        RegisterPlaced(pair.tower);

        UICanvasController.TowerSpawnpoints.Remove(gameObject);
        Destroy(gameObject);
    }

    private static int GetScaledCost(Tower towerData)
    {
        // If scaler isn't in the scene, fall back to base price (safe behavior)
        if (TowerPriceScaler.Instance == null)
            return towerData.price;

        return TowerPriceScaler.Instance.GetCurrentPrice(towerData);
    }

    private static void RegisterPlaced(Tower towerData)
    {
        if (TowerPriceScaler.Instance == null)
            return;

        TowerPriceScaler.Instance.RegisterPlaced(towerData);
    }
}
