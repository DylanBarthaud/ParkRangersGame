using System.Collections;
using UnityEngine;

public class Flare : Item
{
    [Header("Flare Settings")]
    private GFXHandler gfx;
    [SerializeField] private float duration = 5; 

    private void Awake()
    {
        gfx = GetComponent<GFXHandler>();
    }

    public override void UseItem(GameObject user)
    {
        gfx.EnableGFXServerRpc("Particles");
        SetCanPickUpItemServerRPC(false);
        user.GetComponent<Inventory>().RemoveItem(this);
        DropItem(user.transform.position, null); 
    }

    public override void DropItem(Vector3 newPos, Inventory inventory)
    {
        base.DropItem(newPos, inventory);
        StartCoroutine(DeleteFlare(duration)); 
    }

    private IEnumerator DeleteFlare(float duration)
    {
        yield return new WaitForSeconds(duration);
        DespawnItemServerRPC(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster")) 
            DespawnItemServerRPC();
    }
}
