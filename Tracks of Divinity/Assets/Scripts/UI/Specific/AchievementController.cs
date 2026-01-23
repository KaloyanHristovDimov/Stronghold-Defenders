using TMPro;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI achievementName, achievementDescription;
    [SerializeField] private GameObject lockedGO;

    public void Setup(Achievement achievement)
    {
        achievementName.text = achievement.name;
        achievementDescription.text = achievement.description;
        
        if(!AchievementsLoader.obj.unlockedAchievements.Contains(achievement)) lockedGO.SetActive(true);
    }
}
