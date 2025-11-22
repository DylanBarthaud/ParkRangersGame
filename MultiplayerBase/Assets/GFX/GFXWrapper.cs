using System.Collections.Generic;
using UnityEngine;

public class GFXWrapper : MonoBehaviour
{
    [SerializeField] private GameObject[] gfx;
    [SerializeField] private string gfxName;
    public string GfxName => gfxName; 

    public void EnableGFX()
    {
        foreach (var gfx in gfx)
        {
            if (gfx != null) gfx.SetActive(true); 
            else Debug.LogError($"{GfxName} is NULL");
        }
    }

    public void DisableGFX()
    {
        foreach (var gfx in gfx)
        {
            if (gfx != null) gfx.SetActive(false);
            else Debug.LogError($"{GfxName} is NULL");
        }
    }
}
