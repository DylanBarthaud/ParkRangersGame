using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(fileName = "New Footstep Collection", menuName = "Create New Footstep Collection")]
public class FootstepsCollection : ScriptableObject
{
    public AudioResource footstepSounds;
    public AudioClip jumpSound;
    public AudioClip landSound;
}
