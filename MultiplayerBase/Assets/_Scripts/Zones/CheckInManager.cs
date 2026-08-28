using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CheckInManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject zoneCardPrefab;
    [SerializeField] private Transform zoneCardContainer; 
    private Interactor interactor;

    private List<GameObject> currentZoneCards = new List<GameObject>();

    private CheckInTable table; 

    public void OpenMenu(CheckInTable table)
    {
        uiPanel.SetActive(true);
        this.table = table;
    }

    public void CloseMenu()
    {
        table.SetCanInteractServerRPC(true);

        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
            playerController.canInteract = true;
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
}