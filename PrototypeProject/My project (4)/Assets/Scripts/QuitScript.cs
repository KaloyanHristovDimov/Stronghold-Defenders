using UnityEngine;

public class QuitScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
