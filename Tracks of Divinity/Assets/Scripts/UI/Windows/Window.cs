using UnityEngine;

public class Window : MonoBehaviour
{
    public void Activate(bool active = true) => gameObject.SetActive(active);

    public virtual void Open() => Activate(true);
    public virtual void Close() => Activate(false);

    public virtual void CloseIfOpen(){ if(gameObject.activeSelf) Close(); }
    public virtual void Update(){ if(Input.GetMouseButtonDown(1)) CloseIfOpen(); }
}
