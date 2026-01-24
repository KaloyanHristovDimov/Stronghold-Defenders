using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterController : MonoBehaviour
{
    [SerializeField] private int startingCount = 0;
    [SerializeField] private List<TextMeshProUGUI> counterTexts;

    protected int count;

    public int Count => count;

    protected void UpdateText() => counterTexts.ForEach(t => t.text = count.ToString());

    private void Awake() => SetCount(startingCount);

    public void SetCount(int value)
    {
        count = value;
        UpdateText();
    }

    public void IncrementCount(int value = 1)
    {
        count += value;
        UpdateText();
    }

    public void DecrementCount(int value = 1)
    {
        count -= value;
        UpdateText();
    }
}
