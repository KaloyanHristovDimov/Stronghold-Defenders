using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceWindow : Window
{
    public static ChoiceWindow Instance;
    ///<summary> indexes = id (like left is 0, mid is 1, right as 2) </summary>
    [SerializeField] private List<Transform> tileContainers;
    ///<summary> indexes = id (like left is 0, mid is 1, right as 2) </summary>
    [SerializeField] private List<TextMeshProUGUI> towerSpacesTexts, biomeBuffTexts, biomeDebuffTexts, tileNameTexts;

    const string towerSpacesTextPrefix = "<color=#666666>Tower Spaces:</color> ",
     biomeBuffTextPrefix = "<color=#666666>Biome Buff:</color> ", biomeDebuffTextPrefix = "<color=#666666>Biome Debuff:</color> ";
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

    }

    private readonly List<GameObject> spawnedIcons = new();
    private List<TileData> tilesToChooseFrom = new List<TileData>();
    private Action<TileData> Choose;


    public void Open(List<TileData> tilesToShow, Action<TileData> onChosen)
    {
        tilesToChooseFrom = tilesToShow;
        Choose = onChosen;
        // Additional logic for opening the ChoiceWindow can be added here
        // Like assigning randomly hosen tiles to their respective tileContainers ^ & v
        for (int i = 0; i < tileContainers.Count; i++)
        {
            // For testing purposes, instantiate a temporary placeholder tile, remove when adding the actual recieved tiles
            // Can add a new Open() with arguments List<GameObject> with the chosen tiles and simply instantiate them here
            // Have to make the default 0 argument Open() throw an exception when going for that route
            var iconPrefab = tilesToShow[i].icon;
            var iconInstance = Instantiate(iconPrefab, tileContainers[i]);
            spawnedIcons.Add(iconInstance);

            tileNameTexts[i].text = tilesToShow[i].name;
            towerSpacesTexts[i].text = towerSpacesTextPrefix + tilesToShow[i].towerAmount.ToString();
            switch (tilesToShow[i].tileBiome)
            {
                case TileData.biomeType.Plains:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Cheaper than normal";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "None";
                    break;
                case TileData.biomeType.Desert:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Higher range";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "Lower attack speed";
                    break;
                case TileData.biomeType.Snow:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Towers slow enemies";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "Lower range";
                    break;
                case TileData.biomeType.Volcanic:
                    biomeBuffTexts[i].text = biomeBuffTextPrefix + "Higher damage";
                    biomeDebuffTexts[i].text = biomeDebuffTextPrefix + "More expensive";
                    break;
                default:
                    break;
            }

            // Also assign the stats from the chosen tiles to the respective texts here
            // Do it like this for each field (3): towerSpacesTexts[i].text = towerSpacesTextPrefix + yourObjectWithTileInfo.towerSpaces;
        }

        Activate(true); //this should always be at the end of this method
    }
    public void ChooseOption(int id)
    {
        Close(); //this should always be at the start of this method, unless you wanna spawn the tiles without the player noticing
        Debug.Log($"Option {id} chosen.");
        Choose(tilesToChooseFrom[id]);
        // Logic for handling the chosen option, use the id (left is 0, mid is 1, right as 2) to know which was chosen
    }

    public override void Close()
    {
        base.Close();
        for (int i = 0; i < tileContainers.Count; i++) 
        {
            if (spawnedIcons[i] != null)
                Destroy(spawnedIcons[i]);
        }
        spawnedIcons.Clear();
    }
}
