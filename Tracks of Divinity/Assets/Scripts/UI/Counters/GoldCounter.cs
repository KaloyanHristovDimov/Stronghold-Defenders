using UnityEngine;
using TMPro;

public class GoldCounter : CounterController
{
    public bool CanAfford(int cost) => Count >= cost;
}