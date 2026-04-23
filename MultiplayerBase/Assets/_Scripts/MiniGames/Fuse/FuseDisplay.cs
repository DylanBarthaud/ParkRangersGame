using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FuseDisplay : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] lights;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private TextMeshProUGUI leverValuesText; 

    [ServerRpc(RequireOwnership = false)]
    public void SetLightsServerRpc(bool[] isActiveArray) => SetLightsClientRpc(isActiveArray);

    [ClientRpc]
    private void SetLightsClientRpc(bool[] isActiveArray)
    {
        Array.Reverse(isActiveArray);
        int i = 0;
        foreach (var light in lights)
        {
            if (isActiveArray[i]) light.material = onMaterial;
            else light.material = offMaterial;
            i++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetLeverValuesKeyTextServerRpc(uint[] binaryValues) => SetLeverValuesKeyTextClientRpc(binaryValues);

    [ClientRpc]
    private void SetLeverValuesKeyTextClientRpc(uint[] binaryValues)
    {
        int i = 0; 
        foreach(var value  in binaryValues)
        {
            leverValuesText.text += $"\n Lever {i + 1} value: {Convert.ToString(binaryValues[i], 2).PadLeft(6, '0')}";
            i++; 
        }
    }
}
