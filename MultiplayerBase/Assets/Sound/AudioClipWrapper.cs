using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipWrapper", menuName = "Scriptable Objects/AudioClipWrapper")]
public class AudioClipWrapper : ScriptableObject
{
    public AudioClip clip;
    public string clipName; 
    public int sourceId; 
}
