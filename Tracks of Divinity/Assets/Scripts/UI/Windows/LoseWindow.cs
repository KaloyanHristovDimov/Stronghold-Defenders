using TMPro;
using UnityEngine;

public class LoseWindow : PauseMenuWindow
{
    [SerializeField] private TextMeshProUGUI highscoreCount;

    public override void Close() => Debug.Log("LoseWindow: Shouldn't call Close() unless the player right-clicked.");

    private void Awake()
    {
        if(AchievementsLoader.obj.highScore < UICanvasController.WaveCounter.Count)
            AchievementsLoader.obj.highScore = UICanvasController.WaveCounter.Count;

        highscoreCount.text = AchievementsLoader.obj.highScore.ToString();
    }
}
