using Unity.Netcode;
using UnityEngine;

public class KeypadUniqueCode : NetworkBehaviour
{
    private int[] code; 
    public int[] Code => code;

    public override void OnNetworkSpawn()
    {
        
    }
}
