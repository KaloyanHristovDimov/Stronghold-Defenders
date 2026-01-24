using System.Collections.Generic;
using UnityEngine;

public class AchievementsWindow : Window
{
    [SerializeField] private Transform achievementsContainer;
    [SerializeField] private GameObject achievementPrefab;

    private void CreateAchievement(Achievement a) => Instantiate(achievementPrefab, achievementsContainer).GetComponent<AchievementController>().Setup(a);

    public override void Open()
    {
        if(AchievementsLoader.obj.unlockedAchievements.Count + AchievementsLoader.obj.lockedAchievements.Count != AchievementsLoader.Source.Count)
            AchievementsLoader.obj.InitialSet();
        AchievementsLoader.obj.unlockedAchievements.ForEach(a => CreateAchievement(a));
        AchievementsLoader.obj.lockedAchievements.ForEach(a => CreateAchievement(a));
        
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        foreach(Transform child in achievementsContainer) Destroy(child.gameObject);
    }
}
