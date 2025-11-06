using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    private static bool initialized = false;

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);

        LoadMenu();
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }
}
