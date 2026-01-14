using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawn : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject towerPrefab1;
    public GameObject towerPrefab2;
    public GameObject towerPrefab3;

    public int tower1Cost = 50;
    public int tower2Cost = 100;
    public int tower3Cost = 75;

    public GameObject TowerSelector;

    public ParticleSystem errorParticles;
    public AudioSource audioSource;


    void Start()
    {
        TowerSelector = GameObject.Find("TowerSelector");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (TowerSelector.GetComponent<TowerSelect>().selectedTower)
        {
            case 1:
                TrySpawnTower(towerPrefab1, tower1Cost);
                break;
            case 2:
                TrySpawnTower(towerPrefab2, tower2Cost);
                break;
            case 3:
                TrySpawnTower(towerPrefab3, tower3Cost);
                break;
            default:
                ErrorFeedback();
                break;
        }
        
    }


    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

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
        TutorialManager.Instance.OnPlateSelected();
        Destroy(gameObject);
    }

    private void ErrorFeedback()
    {
        Debug.Log("Not enough money or no tower selected");
        errorParticles.Play();
        audioSource.Play();
    }

  
}
