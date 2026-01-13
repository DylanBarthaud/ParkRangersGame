using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Crow_AI : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void SetNoiseData(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(0.95f, 1.0f);
        audioSource.volume = Random.Range(0.1f, 0.2f);
        audioSource.panStereo = Random.Range(-1.0f, 1.0f);
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length + 0.2f);
    }
}
