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

    [ClientRpc]
    public void EnableGFXClientRpc(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].EnableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    [ClientRpc]
    public void DisableGFXClientRpc(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName)) gfxDictonary[gfxName].DisableGFX();
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }
}