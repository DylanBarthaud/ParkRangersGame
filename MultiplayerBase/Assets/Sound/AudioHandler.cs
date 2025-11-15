using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSourceArray;
    [SerializeField] AudioClipWrapper[] wrappedAudioClipArray;

    private Dictionary<string, AudioClipWrapper> audioClipDictonary;
    private Dictionary<AudioClipWrapper, AudioSource> audioSourceDictonary;

    private void Awake()
    {
        audioClipDictonary = new Dictionary<string, AudioClipWrapper>();
        audioSourceDictonary = new Dictionary<AudioClipWrapper, AudioSource>(); 

        foreach (AudioClipWrapper wrappedClip in wrappedAudioClipArray)
        {
            audioClipDictonary.Add(wrappedClip.clipName, wrappedClip);
            audioSourceDictonary.Add(wrappedClip, audioSourceArray[wrappedClip.sourceId]); 
        }
    }

    public void PlaySound(string clipName, bool loop = false, float minDist = 5, float maxDist = 100)
    {
        AudioSource audioSource;
        AudioClip audioClip;

        (audioSource, audioClip) = GetSourceAndClip(clipName);

        audioSource.loop = loop;
        audioSource.minDistance = minDist;
        audioSource.maxDistance = maxDist;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopPlayingClipSound(string clipName)
    {
        AudioSource audioSource;
        AudioClip audioClip;

        (audioSource, audioClip) = GetSourceAndClip(clipName);

        if (audioSource.clip == audioClip) audioSource.Stop();
    }

    public void StopSoundFromAudioSource(int sourceId)
    {
        AudioSource audioSource = audioSourceArray[sourceId]; 
        if(audioSource.isPlaying) audioSource.Stop();
    }

    private (AudioSource, AudioClip) GetSourceAndClip(string clipName)
    {
        if (audioClipDictonary.ContainsKey(clipName))
        {
            AudioClipWrapper wrappedClip = audioClipDictonary[clipName];
            if (audioSourceDictonary.ContainsKey(wrappedClip))
            {
                AudioSource audioSource = audioSourceDictonary[wrappedClip];
                AudioClip audioClip = wrappedClip.clip;
                
                return (audioSource, audioClip);
            }
            else Debug.LogError($"AudioSourceDictonary does not contain {wrappedClip} as a key");
        }
        else Debug.LogError($"AudioClipDictonary does not contain {clipName} as a key");

        return (null, null);
    }
}
