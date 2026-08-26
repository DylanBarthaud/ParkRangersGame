using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void OpenWebURL(string url)
    {
        Application.OpenURL(url);
    }
}
