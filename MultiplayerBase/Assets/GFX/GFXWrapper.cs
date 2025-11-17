using System.Collections.Generic;
using UnityEngine;

public class GFXWrapper : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] gfx;
    [SerializeField] private string name_;
    public string gfxName => name_; 

    public void EnableGFX()
    {
        foreach (var gfx in gfx)
        {
            if (gfx != null)
            {
                gfx.enabled = true;
            }
            else Debug.LogError($"{gfxName} is NULL");
        }
    }

    public void DisableGFX()
    {
        foreach (var gfx in gfx)
        {
            if (gfx != null)
            {
                gfx.enabled = false;
            }
            else Debug.LogError($"{gfxName} is NULL");
        }
    }
}
