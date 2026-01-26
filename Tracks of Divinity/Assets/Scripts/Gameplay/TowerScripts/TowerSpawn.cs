using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class PrefabPricePair
    {
        public GameObject prefab;
        public int price;
    }

    public List<PrefabPricePair> towers;

    private bool initialized;

    //public AudioSource audioSource;


    public void Initialize()
    {
        UICanvasController.TowerSpawnpoints.Add(gameObject);
        gameObject.SetActive(false);
        initialized = true;
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
        UICanvasController.currentTowerButton.ShowCard();
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
        int selected = UICanvasController.currentTowerButton.id;

        if (!UICanvasController.GoldCounter.CanAfford(towers[selected].price))
        {
            //ErrorFeedback();
            return;
        }


        UICanvasController.GoldCounter.DecrementCount(towers[selected].price);
        Instantiate(towers[selected].prefab, transform.position, Quaternion.identity);
        UICanvasController.TowerSpawnpoints.Remove(gameObject);
        Destroy(gameObject);
    }

    

    //private void ErrorFeedback()
    //{
    //    Debug.Log("Not enough money or no tower selected");
    //    audioSource.Play();
    //}
}
