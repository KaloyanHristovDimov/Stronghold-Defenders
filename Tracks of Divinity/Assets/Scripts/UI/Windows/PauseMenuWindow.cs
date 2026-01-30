using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuWindow : PauseTypeWindow
{
    public void Restart() => SceneManager.LoadScene(1);

    public void MainMenu() => SceneManager.LoadScene(0);

    public void Quit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
