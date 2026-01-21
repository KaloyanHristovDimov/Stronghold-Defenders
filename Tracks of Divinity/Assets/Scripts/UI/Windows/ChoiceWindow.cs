using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceWindow : Window
{
    ///<summary> indexes = id (like left is 0, mid is 1, right as 2) </summary>
    [SerializeField] private List<Transform> tileContainers;
    ///<summary> indexes = id (like left is 0, mid is 1, right as 2) </summary>
    [SerializeField] private List<TextMeshProUGUI> towerSpacesTexts, biomeBuffTexts, biomeDebuffTexts;

    [SerializeField] private GameObject temporaryPlaceholderTestTilePrefab;

    const string towerSpacesTextPrefix = "<color=#666666>Tower Spaces:</color> ",
     biomeBuffTextPrefix = "<color=#666666>Biome Buff:</color> ", biomeDebuffTextPrefix = "<color=#666666>Biome Debuff:</color> ";

    public override void Open()
    {

        // Additional logic for opening the ChoiceWindow can be added here
        // Like assigning randomly hosen tiles to their respective tileContainers ^ & v
        for (int i = 0; i < tileContainers.Count; i++)
        {
            // For testing purposes, instantiate a temporary placeholder tile, remove when adding the actual recieved tiles
            // Can add a new Open() with arguments List<GameObject> with the chosen tiles and simply instantiate them here
            // Have to make the default 0 argument Open() throw an exception when going for that route
            Instantiate(temporaryPlaceholderTestTilePrefab, tileContainers[i]);

            // Also assign the stats from the chosen tiles to the respective texts here
            // Do it like this for each field (3): towerSpacesTexts[i].text = towerSpacesTextPrefix + yourObjectWithTileInfo.towerSpaces;
        }

        Activate(true); //this should always be at the end of this method
    }

    public void ChooseOption(int id)
    {
        Close(); //this should always be at the start of this method, unless you wanna spawn the tiles without the player noticing

        // Logic for handling the chosen option, use the id (left is 0, mid is 1, right as 2) to know which was chosen
        Debug.Log($"Option {id} chosen.");
    }
}
