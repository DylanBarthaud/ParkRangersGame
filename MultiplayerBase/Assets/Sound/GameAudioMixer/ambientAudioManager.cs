using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class ambientAudioManager : MonoBehaviour
{
    public GameObject audioCenter;
    public GameObject audioSource;
    public float loadTime = 5f;
    public AudioSource[] closeSources;
    public AudioClip[] closeSounds;
    public AudioSource[] farSources;
    int currentAudioId;
    int currentSoundId;
    float rotationIncrement;
    AudioClip soundToPlay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(MoveSource());

    }


    IEnumerator MoveSource()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 40));
            


            currentAudioId = Random.Range(0, closeSources.Length);
            currentSoundId = Random.Range(0, closeSounds.Length);

            soundToPlay = closeSounds[currentSoundId];
            closeSources[currentAudioId].PlayOneShot(soundToPlay);
        }
        
    }

    private void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, audioCenter.transform.rotation.z * -1.0f);
    }

}
