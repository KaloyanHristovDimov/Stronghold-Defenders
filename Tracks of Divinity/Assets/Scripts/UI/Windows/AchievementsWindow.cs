using UnityEngine;

public class AchievementsWindow : Window
{
    [SerializeField] private Transform achievementsContainer;
    [SerializeField] private GameObject achievementPrefab;

    private void CreateAchievement(Achievement a) => Instantiate(achievementPrefab, achievementsContainer).GetComponent<AchievementController>().Setup(a);

    private void Start()
    {
        AchievementsLoader.obj.unlockedAchievements.ForEach(a => CreateAchievement(a));
        AchievementsLoader.obj.lockedAchievements.ForEach(a => CreateAchievement(a));
        Close();
    }
}
