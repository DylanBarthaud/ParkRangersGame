using Unity.Netcode;
using UnityEngine;

public class Campfire : NetworkBehaviour, IInteractable
{
    [SerializeField] MeshRenderer logs; 
    [SerializeField] Material putOutMat;

    [SerializeField] ParticleSystem ps;

    [SerializeField] AudioSource audioSource;

    [SerializeField] GFXHandler gFXHandler; 
    private bool putOut = false;

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        if (itemUsed == ItemType.WaterBucket && !putOut)
        {
            return (true, "");
        }
        return (false, $"Requires item: {ItemType.WaterBucket}");
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        EventManager.instance.OnPuzzleComplete();
        PutOutCampfireServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PutOutCampfireServerRpc()
    {
        PutOutCampfireClientRpc();
        gFXHandler.DisableGFXServerRpc("FireGFX");
    }

    [ClientRpc]
    private void PutOutCampfireClientRpc()
    {
        //logs.material = putOutMat;
        ps.Stop();
        audioSource.Stop();
        putOut = true;
    }

}
