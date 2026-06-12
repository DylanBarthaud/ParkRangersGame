using UnityEngine;

public class CheckInManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private Interactor interactor;
    private CheckInTable currentTable; 

    public void OpenMenu() => uiPanel.SetActive(true);
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
    }

    public void SetInteractor(Interactor interactor) => this.interactor = interactor;
    public void SetTable(CheckInTable table) => this.currentTable = table;
    public void CheckIn() => interactor.SetZone(currentTable.Zone);
    public void CompleteZone() => currentTable.CompleteZone();
}
