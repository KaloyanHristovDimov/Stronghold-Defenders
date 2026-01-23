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



    //public AudioSource audioSource;


    void Start()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int selected = TowerSelect.Instance.selectedTower;

        if (selected < 0 || selected >= towers.Count)
        {
            //ErrorFeedback();
            return;
        }

        TrySpawnTower(towers[selected].prefab, towers[selected].price);
    }


    private void TrySpawnTower(GameObject prefab, int cost)
    {
        if (!MoneyManager.Instance.CanAfford(cost))
        {
            //ErrorFeedback();
            return;
        }

        MoneyManager.Instance.SpendMoney(cost);
        Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    //private void ErrorFeedback()
    //{
    //    Debug.Log("Not enough money or no tower selected");
    //    audioSource.Play();
    //}
}
