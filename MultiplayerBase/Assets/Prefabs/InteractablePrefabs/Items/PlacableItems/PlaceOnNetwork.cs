using Unity.Netcode;
using UnityEngine;

public class PlaceOnNetwork : MonoBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SpawnObjectOnServerRpc(Vector3 position)
    {
        GetComponent<NetworkObject>().Spawn();
        SpawnObjectOnClientRpc(position);
    }

    [ClientRpc]
    private void SpawnObjectOnClientRpc(Vector3 position) => transform.position = position;
}
