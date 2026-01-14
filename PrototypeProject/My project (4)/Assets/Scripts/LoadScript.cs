using UnityEngine;

public class LoadScript : MonoBehaviour
{
    public string SceneName;

    public void LoadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
