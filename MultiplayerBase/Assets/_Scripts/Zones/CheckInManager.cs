using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckInManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject zoneCardPrefab;
    [SerializeField] private Transform zoneCardContainer; 
    private Interactor interactor;

    private List<GameObject> currentZoneCards = new List<GameObject>();

    public void OpenMenu()
    {
        uiPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
            interactor.GetComponent<Inventory>().EnableInv();
        }

        Cursor.lockState = CursorLockMode.Locked;

        uiPanel.SetActive(false);
        foreach (GameObject zoneCard in currentZoneCards) Destroy(zoneCard);
    }

    public void AddZoneCard(ZoneInfo zoneInfo, Interactor interactor)
    {
        ZoneInfoCard newZoneCard = Instantiate(zoneCardPrefab, zoneCardContainer).GetComponent<ZoneInfoCard>();
        currentZoneCards.Add(newZoneCard.gameObject);
        newZoneCard.Initilize(zoneInfo, interactor);
        this.interactor = interactor;
    }

    #region old shii
    /*
    public void SetInteractor(Interactor interactor) => this.interactor = interactor;
    public void SetTable(CheckInTable table) => currentTable = table;
    public void SetZone(Zones zone) => tableZone = zone;
    public void TableCheckIn() => EventManager.instance.OnCheckIn(interactor.OwnerClientId, tableZone);
    public void CompleteZone() => currentTable.CompleteZone();
    public void CheckIn(int zone)
    {
        EventManager.instance.OnCheckIn(interactor.OwnerClientId, (Zones)zone); 
        UpdateTxt();
    }

    private void UpdateTxt()
    {
        zoneTxt.text = $"Zone: {currentTable.Zone}";
        playersCheckedInTxt.text = $"Checked in: {currentTable.CheckedInPlayers.Count}";
        tasksLeftTxt.text = $"Tasks Left: {currentTable.TasksInZone - currentTable.TasksComplete} / {currentTable.TasksInZone}";
    }

    public void OpenHubUI()
    {
        hubUI.SetActive(true);
    }
    */
    #endregion

}