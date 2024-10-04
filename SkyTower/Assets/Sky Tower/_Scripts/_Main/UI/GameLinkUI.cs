using UnityEngine;

public class GameLinkUI : MonoBehaviour
{

    public void OpenURL(string address)
    {
        Application.OpenURL(address);
    }
}
