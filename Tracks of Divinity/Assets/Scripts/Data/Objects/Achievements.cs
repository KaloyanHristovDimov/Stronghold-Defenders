using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class Achievements
{
    public List<Achievement> unlockedAchievements = new(), lockedAchievements = new();

    [JsonConstructor]
    public Achievements(){}

    public void InitialSet()
    {
        unlockedAchievements = new(){ AchievementsLoader.AchievementsSource[0] };
        lockedAchievements = new(AchievementsLoader.AchievementsSource);
        lockedAchievements.RemoveAt(0);
    }
}
