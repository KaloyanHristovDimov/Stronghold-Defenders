using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class TileChoice : MonoBehaviour
{
    public static TileChoice Instance;

    [SerializeField] Button[] buttons;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

    }

    public void ShowChoices(List<TileData> tiles, Action<TileData> onChosen)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < tiles.Count)
            {
                TileData tile = tiles[i];

                buttons[i].gameObject.SetActive(true);
                TMP_Text label = buttons[i].GetComponentInChildren<TMP_Text>();
                label.text = tile.name;

                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);
                    onChosen(tile);
                });
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
}
