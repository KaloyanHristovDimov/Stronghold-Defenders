using UnityEngine;

public class Window : MonoBehaviour
{
    public void ActivateParent(bool active = true) => transform.parent.gameObject.SetActive(active);

    public virtual void Open() => ActivateParent(true);
    public virtual void Close() => ActivateParent(false);

    public virtual void CloseIfOpen(){ if(transform.parent.gameObject.activeSelf) Close(); }
}
