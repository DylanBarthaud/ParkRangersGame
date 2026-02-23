using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FuseDisplay : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] lights;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    [ServerRpc(RequireOwnership = false)]
    public void SetLightsServerRpc(bool[] isActiveArray) => SetLightsClientRpc(isActiveArray);

    [ClientRpc]
    private void SetLightsClientRpc(bool[] isActiveArray)
    {
        int i = 0;
        foreach (var light in lights)
        {
            if (isActiveArray[i]) light.material = onMaterial;
            else light.material = offMaterial;
            i++;
        }
    }
}
