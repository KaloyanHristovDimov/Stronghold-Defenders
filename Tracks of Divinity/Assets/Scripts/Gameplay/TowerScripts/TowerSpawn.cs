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

    //public AudioSource audioSource;


    public void Initialize(int tileBiome)
    {
        UICanvasController.TowerSpawnpoints.Add(gameObject);
        gameObject.SetActive(false);
        initialized = true;
        biome = tileBiome;
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     int selected = TowerSelect.Instance.selectedTower;

    //     if (selected < 0 || selected >= towers.Count)
    //     {
    //         //ErrorFeedback();
    //         return;
    //     }

    //     TrySpawnTower(towers[selected].prefab, towers[selected].price);
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!initialized)
            return;
        UICanvasController.currentTowerSpawnPoint = this;
        UICanvasController.currentTowerButton.ShowCard(biome, (int)UICanvasController.currentTowerButton.type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!initialized)
            return;
        UICanvasController.currentTowerSpawnPoint = null;
        UICanvasController.currentTowerButton.HideCard();
    }

    public void TrySpawnTower()
    {
        TowerPricePair pair = UICanvasController.Towers[biome].pair[(int)UICanvasController.currentTowerButton.type];
        OnPointerExit(null);

        if (!UICanvasController.GoldCounter.CanAfford(pair.price))
        {
            //ErrorFeedback();
            return;
        }


        UICanvasController.GoldCounter.DecrementCount(pair.price);
        Instantiate(pair.prefab, transform.position, Quaternion.identity);
        UICanvasController.TowerSpawnpoints.Remove(gameObject);
        Destroy(gameObject);
    }

    

    //private void ErrorFeedback()
    //{
    //    Debug.Log("Not enough money or no tower selected");
    //    audioSource.Play();
    //}
}
