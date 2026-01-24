using UnityEngine;

public class PauseTypeWindow : Window
{
    public override void Open()
    {
        Time.timeScale = 0f;
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1f;
    }
}
