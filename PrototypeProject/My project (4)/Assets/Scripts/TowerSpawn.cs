using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;



public class TowerSpawn : MonoBehaviour, IPointerDownHandler
{
    [System.Serializable]
    public class PrefabPricePair
    {
        public GameObject prefab;
        public int price;
    }
    
    public List<PrefabPricePair> towers;

    public GameObject TowerSelector;

    public AudioSource audioSource;


    void Start()
    {
        TowerSelector = GameObject.Find("TowerSelector");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < towers.Count; i++) 
        {
            if (TowerSelector.GetComponent<TowerSelect>().selectedTower == i) 
            {
                TrySpawnTower(towers[i - 1].prefab, towers[i - 1].price);
                return;
            }
        }
        ErrorFeedback();
    }


    private void TrySpawnTower(GameObject prefab, int cost)
    {
        if (!MoneyManager.Instance.CanAfford(cost))
        {
            ErrorFeedback();
            return;
        }

        MoneyManager.Instance.SpendMoney(cost);
        Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ErrorFeedback()
    {
        Debug.Log("Not enough money or no tower selected");
        audioSource.Play();
    }
}
