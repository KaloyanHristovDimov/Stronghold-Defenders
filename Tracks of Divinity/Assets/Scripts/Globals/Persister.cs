using UnityEngine;

public class Persister : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTex, dragCursorTex;

    public static Persister Instance { get; private set; }

    public static Vector2 cursorHotspot;
    public static Achievements achievementsObj;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cursorHotspot = new(Instance.cursorTex.width / 3f, Instance.cursorTex.height / 4f);
        DontDestroyOnLoad(gameObject);
    }

    public static void SetCursor(bool isDragging = false)
    {
        if(isDragging) Cursor.SetCursor(Instance.dragCursorTex, cursorHotspot, CursorMode.Auto);
        else Cursor.SetCursor(Instance.cursorTex, cursorHotspot, CursorMode.Auto);
    }
}
