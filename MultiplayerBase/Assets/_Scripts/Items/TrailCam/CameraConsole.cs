using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraConsole : MonoBehaviour, IInteractable
{
    private List<TrailCamera> cameras = new List<TrailCamera>();
    private int currentIndex = 0; 

    private void Awake()
    {
        EventManager.instance.onTrailCameraPlaced += OnTrailCameraPlaced;
    }

    private void OnTrailCameraPlaced(TrailCamera camera)
    {
        Debug.Log("Placed");

        cameras.Add(camera);
        if(cameras.Count == 1) camera.ActivateCamera();
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        Debug.Log("HERE"); 

        cameras[currentIndex].DeactivateCamera();

        currentIndex++;
        if(currentIndex >= cameras.Count) currentIndex = 0;

        cameras[currentIndex].ActivateCamera();
    }
}
