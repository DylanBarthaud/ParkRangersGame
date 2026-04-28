using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class scriptHideForPlayer : NetworkBehaviour
{
    public GameObject meshToHide;
    public bool hideForSelf = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hideForSelf)
        {

            if (IsLocalPlayer)
            {
                meshToHide.SetActive(false);
            }
        }
        else
            if (!IsLocalPlayer)
            {
                meshToHide.SetActive(true);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
