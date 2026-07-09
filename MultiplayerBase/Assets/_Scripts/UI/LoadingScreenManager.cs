using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : NetworkBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] public Image loadingBarFill;
    public RandomSelectBackground ImageRandomizer;
    public RandomTipSelector TipRandomizer;
    
    private AsyncOperation currentAsyncOp;
    private bool trackSceneProgress = false;
    private float visualProgress = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
    }

    private void Update()
    {
        if (trackSceneProgress)
        {
            float target = currentAsyncOp.progress / 0.9f;
            target = Mathf.Clamp01(target);

            visualProgress = Mathf.MoveTowards(visualProgress, target, Time.deltaTime * 0.5f);

            loadingBarFill.fillAmount = visualProgress;
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log(sceneEvent.SceneEventType);

        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                loadingScreen.SetActive(true);
                ImageRandomizer.RandomizeImage();
                TipRandomizer.RandomizeTip();
                visualProgress = 0f;
                currentAsyncOp = sceneEvent.AsyncOperation;
                trackSceneProgress = true;
                break;

            case SceneEventType.LoadComplete:
                break;

            case SceneEventType.LoadEventCompleted:
                StartCoroutine(LoadTextures()); 
                break;
        }
    }

    private IEnumerator LoadTextures()
    {
        yield return new WaitForEndOfFrame();
        trackSceneProgress = false;
        loadingScreen.SetActive(false);
    }
}
