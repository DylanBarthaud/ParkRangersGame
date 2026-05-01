using UnityEngine;

public class Flare : Item
{
    [Header("Flare Settings")]
    private GFXHandler gfx;

    private void Awake()
    {
        gfx = GetComponent<GFXHandler>();
    }

    public override void UseItem(GameObject user)
    {
        gfx.EnableGFXServerRpc("Particles");
    }
}
