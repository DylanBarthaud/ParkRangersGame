using Unity.Netcode;
using UnityEngine;

public class TrailCamera : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] RenderTexture renderTexture;

    public override void OnNetworkSpawn()
    {
        EventManager.instance.OnTrailCameraPlaced(this);
    }

    public void ActivateCamera() => ActivateCameraServerRpc();
    public void DeactivateCamera() => DeactivateCameraServerRpc();

    [ServerRpc(RequireOwnership = false)]
    public void ActivateCameraServerRpc() => ActivateCameraClientRpc();

    [ClientRpc]
    public void ActivateCameraClientRpc() => _camera.gameObject.SetActive(true);

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateCameraServerRpc() => DeactivateCameraClientRpc();

    [ClientRpc]
    public void DeactivateCameraClientRpc() => _camera.gameObject.SetActive(false);
}
