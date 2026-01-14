using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TextMeshProUGUI tutorialText;

    [TextArea(2, 5)]
    public string[] tutorialMessages;

    public float autoHideTime = 4f;

    private int currentIndex = 0;
    private bool waitingForTowerSelect = true;
    private bool waitingForPlateSelect = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowMessage(0);
    }

    void ShowMessage(int index)
    {
        tutorialText.text = tutorialMessages[index];
        tutorialText.gameObject.SetActive(true);
    }

    // CALLED BY TOWER BUTTONS
    public void OnTowerSelected()
    {
        if (!waitingForTowerSelect) return;

        waitingForTowerSelect = false;
        NextMessage();
    }
    // CALLED BY Spawning a tower on a plate
    public void OnPlateSelected()
    {
        if (!waitingForPlateSelect) return;

        waitingForPlateSelect = false;
        NextMessage();
    }


    void NextMessage()
    {
        currentIndex++;

        if (currentIndex >= tutorialMessages.Length)
        {
            tutorialText.gameObject.SetActive(false);
            return;
        }

        ShowMessage(currentIndex);
        StartCoroutine(AutoHideAndContinue());
    }

    IEnumerator AutoHideAndContinue()
    {
        yield return new WaitForSeconds(autoHideTime);
        NextMessage();
    }
}
