using Unity.Netcode;
using UnityEngine;

public class SetItemActiveOnTaskComplete : MonoBehaviour, ISetByTask
{
    [SerializeField] GameObject[] objsToSet; 

    public void Set(bool active) => SetServerRpc(active);

    [ServerRpc(RequireOwnership = false)]
    private void SetServerRpc(bool active) => SetClientRpc(active);
    [ClientRpc]
    private void SetClientRpc(bool active)
    {
        foreach (var obj in objsToSet) obj.SetActive(active);
    }
}
