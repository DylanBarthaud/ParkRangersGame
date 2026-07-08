using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneInfoCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI zoneText; 
    [SerializeField] TextMeshProUGUI tasksText;
    [SerializeField] Image zoneIcon;
    [SerializeField] Image background; 

    private ZoneInfo currentInfo;
    private Interactor currentInteractor;

    private void Awake()
    {
        EventManager.instance.onPuzzleComplete += AddPuzzleComplete;
    }

    private void AddPuzzleComplete(bool success, IInteractable interactable)
    {
        if(success)
        {
            currentInfo.TasksComplete++; 
            updateCard();
        }
    }

    public void Initilize(ZoneInfo zoneInfo, Interactor interactor)
    {
        currentInfo = zoneInfo; 
        currentInteractor = interactor;
        updateCard();
    }

    public void CheckIn()
    {
        if (currentInfo.playersIDs.Contains(currentInteractor.OwnerClientId)) return; 

        currentInfo.NumberOfPlayers++;
        currentInfo.playersIDs.Add(currentInteractor.OwnerClientId); 
        EventManager.instance.OnCheckIn(currentInteractor.OwnerClientId, currentInfo.Zone);
        currentInteractor.SetZone(currentInfo.Zone);
    }

    public void CompleteZone()
    {
        if (currentInfo.TasksComplete >= currentInfo.TasksNeeded)
        {
            currentInfo.zoneIsComplete = true;
            background.color = Color.green;
            EventManager.instance.OnZoneComplete();
        }
    }

    private void updateCard()
    {
        zoneText.text = $"Zone: {currentInfo.Zone}";
        tasksText.text = $"Tasks Complete: {currentInfo.TasksComplete}/{currentInfo.TasksNeeded}";
        zoneIcon.sprite = currentInfo.image;
    }
}