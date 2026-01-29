using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class Achievements
{
    public int highScore;
    public List<Achievement> unlockedAchievements = new(), lockedAchievements = new();

    [JsonConstructor]
    public Achievements(){}

    public void InitialSet()
    {
        unlockedAchievements = new(){ AchievementsLoader.Source[0] };
        lockedAchievements = new(AchievementsLoader.Source);
        lockedAchievements.RemoveAt(0);
    }
}
