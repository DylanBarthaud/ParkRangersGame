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
}