using System.Collections.Generic;
using UnityEngine;

public class GFXWrapper : MonoBehaviour
{
    [SerializeField] private GameObject[] gfx;
    [SerializeField] private string name_;
    public string gfxName => name_; 

    public void EnableGFX()
    {
        foreach (var gfx in gfx)
        {
            if (gfx != null)
            {
                gfx.SetActive(true); 
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
                gfx.SetActive(false);
            }
            else Debug.LogError($"{gfxName} is NULL");
        }
    }
}
