using UnityEngine;

public class DontDestoryOnLoad : MonoBehaviour
{
    private static DontDestoryOnLoad instance;
    public GameObject gameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else { instance = this; }


        DontDestroyOnLoad(gameObject);
    }
}
