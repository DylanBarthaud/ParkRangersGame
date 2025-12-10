using Unity.Netcode;
using UnityEngine;

public class MultiplayerAudioHandlerWrapper : NetworkBehaviour
{
    [SerializeField] AudioHandler audioHandler;

    public override void OnNetworkSpawn()
    {
        if(!audioHandler.HasSetUp) audioHandler.SetUp();
    }

    [ClientRpc]
    public void PlaySoundClientRpc(string clipName, bool loop = false, float volume = 1, float minDist = 5, float maxDist = 100, float pitch = 1)
    {
        audioHandler.PlaySound(clipName, loop, volume, minDist, maxDist, pitch);
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
