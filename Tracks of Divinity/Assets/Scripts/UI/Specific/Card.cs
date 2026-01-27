using UnityEngine;

public interface ICardInfo{}

public class Card : MonoBehaviour
{
    protected const string cBeginTag = "<color=#666666>", cEndTag = "</color>";

    public virtual void Display(ICardInfo info)
    {
        
    }
}
