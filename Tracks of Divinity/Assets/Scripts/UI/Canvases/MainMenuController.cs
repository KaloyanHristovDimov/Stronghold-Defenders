using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private void Awake() => Persister.SetCursor();

    public void StartGame() => UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
