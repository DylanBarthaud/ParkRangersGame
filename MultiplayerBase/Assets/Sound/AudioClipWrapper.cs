using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipWrapper", menuName = "Scriptable Objects/AudioClipWrapper")]
public class AudioClipWrapper : ScriptableObject
{
    public AudioResource resource;
    public string clipName; 
    public int sourceId; 
}
