using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneInfoCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI zoneText; 
    [SerializeField] TextMeshProUGUI tasksText;
    [SerializeField] TextMeshProUGUI playersText;
    [SerializeField] Image zoneIcon;
    [SerializeField] Button checkInButton;
    [SerializeField] Button completeZoneButton; 

    private ZoneInfo currentInfo;
    private Interactor currentInteractor; 

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
            EventManager.instance.OnZoneComplete();
        }
    }

    private void updateCard()
    {
        zoneText.text = $"Zone: {currentInfo.Zone}";
        tasksText.text = $"Tasks Complete: {currentInfo.TasksComplete}/{currentInfo.TasksNeeded}";
        playersText.text = $"Players In Zone ({currentInfo.NumberOfPlayers}/{currentInfo.MaxPlayers}):";
        for (int i = 0; i < currentInfo.playersNames.Count; i++) playersText.text += $"\n-{currentInfo.playersNames[i]}";
        zoneIcon.sprite = currentInfo.image;
    }
}