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
    public void EnableGFXServerRpc(string gfxName)
    {
        EnableGFXClientRpc(gfxName);
    }

    [ClientRpc]
    private void EnableGFXClientRpc(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].EnableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisableGFXServerRpc(string gfxName)
    {
        DisableGFXClientRpc(gfxName);
    }

    [ClientRpc]
    private void DisableGFXClientRpc(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].DisableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }
}