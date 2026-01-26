using TMPro;
using UnityEngine;

public class LoseWindow : PauseMenuWindow
{
    [SerializeField] private TextMeshProUGUI highscoreCount;

    public override void Close() => Debug.Log("LoseWindow: Shouldn't call Close() unless the player right-clicked.");

    //Get highscore from Persister and display it
}
