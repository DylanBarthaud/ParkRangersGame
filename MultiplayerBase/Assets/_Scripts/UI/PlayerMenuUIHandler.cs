using UnityEngine;

public class PlayerMenuUIHandler : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool activateMenu = !menuPanel.activeInHierarchy;
            menuPanel.SetActive(activateMenu);
            if(activateMenu)
            {
                prevLockMode = Cursor.lockState;
                prevMovEnabled = characterController.MovementEnabled;

                Cursor.lockState = CursorLockMode.Confined;

                characterController.DisableMovement();
                characterController.GetComponent<Inventory>().DisableInv(); 
            }
            else
            {
                Cursor.lockState = prevLockMode;
                if(prevMovEnabled)
                {
                    characterController.EnableMovement();
                    characterController.GetComponent<Inventory>().EnableInv();
                }
            }
        }
    }
}