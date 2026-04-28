using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class scriptHideForPlayer : NetworkBehaviour
{
    public GameObject meshToHide;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsLocalPlayer)
        {
            meshToHide.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
