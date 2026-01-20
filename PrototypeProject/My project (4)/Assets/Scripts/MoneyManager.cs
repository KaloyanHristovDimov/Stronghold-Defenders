using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{

    public static MoneyManager Instance;

    public int money = 100;

    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        UpdateMoney();
    }

    public bool CanAfford(int cost)
    {
        return money >= cost;
    }

    public void SpendMoney(int cost)
    {
        money -= cost;
        UpdateMoney();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoney();
    }

    void UpdateMoney()
    {
        moneyText.text = "Money: " + MoneyManager.Instance.money;
    }
}