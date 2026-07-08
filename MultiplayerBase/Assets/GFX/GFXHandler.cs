using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GFXHandler : NetworkBehaviour
{
    [SerializeField] private GFXWrapper[] gfxWrapperArray;

    Dictionary<string, GFXWrapper> gfxDictonary = new();

    public override void OnNetworkSpawn()
    {
        foreach (GFXWrapper wrappedGfx in gfxWrapperArray)
        {
            gfxDictonary.Add(wrappedGfx.GfxName, wrappedGfx);
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    public void EnableGFXServerRpc(string gfxName) => EnableGFXClientRpc(gfxName);
    [ClientRpc] private void EnableGFXClientRpc(string gfxName) => EnableGFX(gfxName); 

    [ServerRpc(RequireOwnership = false)]
    public void DisableGFXServerRpc(string gfxName) => DisableGFXClientRpc(gfxName);
    [ClientRpc] private void DisableGFXClientRpc(string gfxName) => DisableGFX(gfxName);

    [ServerRpc(RequireOwnership = false)]
    public void ChangeGFXMaterialServerRpc(string gfxName, int materialIndex) => ChangeGFXMaterialClientRpc(gfxName, materialIndex);
    [ClientRpc] public void ChangeGFXMaterialClientRpc(string gfxName, int materialIndex) => ChangeGFXMaterial(gfxName, materialIndex);

    [ServerRpc(RequireOwnership = false)]
    public void ChangeGFXRenderLayerServerRpc(string gfxName, uint layerMask) => ChangeGFXRenderLayerClientRpc(gfxName, layerMask);
    [ClientRpc] public void ChangeGFXRenderLayerClientRpc(string gfxName, uint layerMask) => ChangeGFXRenderLayer(gfxName, layerMask);

    public void EnableGFX(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].EnableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    public void DisableGFX(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].DisableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    public void ChangeGFXMaterial(string gfxName, int materialIndex)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].ChangeGFXMaterial(materialIndex);
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    public void ChangeGFXRenderLayer(string gfxName, uint layerMask)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].ChangeRenderLayerMask(layerMask);
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }
}