using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private WaitForSeconds _waitForSeconds1 = new(1f);

    private IEnumerator StartGameWithDelay()
    {
        //play animation
        //activate loader and persister
        yield return _waitForSeconds1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }

    public void StartGame() => StartCoroutine(StartGameWithDelay());

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
