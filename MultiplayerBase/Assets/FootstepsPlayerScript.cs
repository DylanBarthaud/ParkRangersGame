using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepsPlayerScript : MonoBehaviour
{

    public FirstPersonController playerRef;
    public AudioResource footstepSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioSource audioSource;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartFootsteps());
    }

    // Update is called once per frame
    private void PlayFootstepSound()
    {
        if (!playerRef.IsGrounded)
        {
            return;
        }
        
        audioSource.resource = footstepSound;
        audioSource.Play();
    }

    IEnumerator StartFootsteps()
    {
        while (true)
            {
                yield return new WaitForSeconds(.5f);
            Debug.Log(playerRef.rb.linearVelocity.magnitude);
                if (playerRef.IsGrounded && playerRef.rb.linearVelocity.magnitude > 1)
                {
                    PlayFootstepSound();

                }
                else
                {
                    yield return null;
                }
            }
        
    }
}
