using Unity.Netcode;
using UnityEngine;

public class PlayerMenuUIHandler : NetworkBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private FirstPersonController characterController;

    private CursorLockMode prevLockMode;
    private bool prevMovEnabled; 

    private void Awake()
    {
        menuPanel.SetActive(false);
    }
     
    private void Update()
    {
        if (!IsOwner) return; 

        if (Cursor.lockState != CursorLockMode.Locked && !menuPanel.activeInHierarchy) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool activateMenu = !menuPanel.activeInHierarchy;
            menuPanel.SetActive(activateMenu);
            if(activateMenu)
            {
                Cursor.lockState = CursorLockMode.Confined;
                characterController.DisableMovement();
                characterController.GetComponent<Inventory>().DisableInv(); 
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                characterController.EnableMovement();
                characterController.GetComponent<Inventory>().EnableInv();
            }
        }
    }
}