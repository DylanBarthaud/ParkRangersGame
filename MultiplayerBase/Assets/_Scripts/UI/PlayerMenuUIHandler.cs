using UnityEngine;

public class PlayerMenuUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private FirstPersonController characterController;

    private void Awake()
    {
        menuPanel.SetActive(false);
    }

    private void Update()
    {
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
