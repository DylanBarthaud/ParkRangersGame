using System.Collections.Generic;
using UnityEngine;

public class GFXHandler : MonoBehaviour
{
    [SerializeField] private GFXWrapper[] gfxWrapperArray;

    Dictionary<string, GFXWrapper> gfxDictonary = new();

    private void Awake()
    {
        foreach(GFXWrapper wrappedGfx in gfxWrapperArray)
        {
            gfxDictonary.Add(wrappedGfx.gfxName, wrappedGfx); 
        }
    }

    public void EnableGFX(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName))
        {
            gfxDictonary[gfxName].EnableGFX();
        }
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }

    public void DisableGFX(string gfxName)
    {
        if (gfxDictonary.ContainsKey(gfxName))
        {
            gfxDictonary[gfxName].DisableGFX();
        }
        else Debug.LogError($"GFX Dictionary does not contain {gfxName} as a key");
    }
}