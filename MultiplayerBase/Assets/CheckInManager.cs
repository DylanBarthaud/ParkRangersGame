using TMPro;
using UnityEngine;

public class CheckInManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI zoneTxt, playersCheckedInTxt, tasksLeftTxt; 
    private Interactor interactor;
    private CheckInTable currentTable;

    public void OpenMenu()
    {
        UpdateTxt(); 
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
    }

    public void SetInteractor(Interactor interactor) => this.interactor = interactor;
    public void SetTable(CheckInTable table) => currentTable = table;
    public void CheckIn()
    {
        currentTable.CheckIn(interactor);
        UpdateTxt();
    }

    public void CompleteZone() => currentTable.CompleteZone();

    private void UpdateTxt()
    {
        zoneTxt.text = $"Zone: {currentTable.Zone}";
        playersCheckedInTxt.text = $"Checked in: {currentTable.CheckedInPlayers.Count}";
        tasksLeftTxt.text = $"Tasks Left: {currentTable.TasksInZone - currentTable.TasksComplete} / {currentTable.TasksInZone}";
    }
}
