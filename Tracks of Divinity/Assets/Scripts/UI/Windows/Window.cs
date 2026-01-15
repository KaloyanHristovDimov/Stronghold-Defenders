using UnityEngine;

public class Window : MonoBehaviour
{
    public void ActivateParent(bool active = true) => gameObject.SetActive(active);

    public virtual void Open() => ActivateParent(true);
    public virtual void Close() => ActivateParent(false);

    public virtual void CloseIfOpen(){ if(gameObject.activeSelf) Close(); }
    public virtual void Update(){ if(Input.GetMouseButtonDown(1)) CloseIfOpen(); }
}
