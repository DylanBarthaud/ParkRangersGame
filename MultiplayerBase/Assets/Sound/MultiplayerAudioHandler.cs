using Unity.Netcode;
using UnityEngine;

public class MultiplayerAudioHandlerWrapper : NetworkBehaviour
{
    [SerializeField] AudioHandler audioHandler;

    public override void OnNetworkSpawn()
    {
        if(!audioHandler.HasSetUp) audioHandler.SetUp();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaySoundServerRpc(string clipName, bool loop = false, float volume = 1, float minDist = 5, float maxDist = 100, float pitch = 1)
    {
        PlaySoundClientRpc(clipName, loop, volume, minDist, maxDist, pitch);
    }

    [ClientRpc]
    private void PlaySoundClientRpc(string clipName, bool loop, float volume, float minDist, float maxDist, float pitch)
    {
        audioHandler.PlaySound(clipName, loop, volume, minDist, maxDist, pitch);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopPlayingClipSoundServerRpc(string clipName)
    {
        StopPlayingClipSoundClientRpc(clipName);
    }

    [ClientRpc]
    public void StopPlayingClipSoundClientRpc(string clipName)
    {
        audioHandler.StopPlayingClipSound(clipName);
    }

    [ClientRpc]
    public void StopSoundFromAudioSourceClientRpc(int sourceId)
    {
        audioHandler.StopSoundFromAudioSource(sourceId);
    }
}
