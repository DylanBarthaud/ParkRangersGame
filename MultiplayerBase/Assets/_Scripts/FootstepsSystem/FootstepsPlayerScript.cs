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

    private bool eventTrigger = true;
    private FootstepSwapper swapper;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swapper = GetComponent<FootstepSwapper>();

        StartCoroutine(StartFootsteps());
    }

    public void SwapFootsteps (FootstepsCollection collection)
    {
        footstepSound = collection.footstepSounds;
        jumpSound = collection.jumpSound;
        landSound = collection.landSound;
    }


    // Update is called once per frame
    private void PlayFootstepSound()
    {
        swapper.CheckLayers();
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
            yield return new WaitForSeconds(.01f);
            
            if (eventTrigger == true)
            {

                if (playerRef.IsGrounded && playerRef.rb.linearVelocity.magnitude > 1 && playerRef.joint.localPosition.magnitude > 0.8f)
                {
                    eventTrigger = false;
                    PlayFootstepSound();
                    Debug.Log("played footstep sound");

                }
                else if (playerRef.joint.localPosition.magnitude < 0.76f)
                {
                    //eventTrigger = true;
                    //Debug.Log("ResetSound");

                }
                else
                {
                    //yield return null;
                }
            }
            else if (playerRef.joint.localPosition.magnitude < 0.76f)
            {
                eventTrigger = true;
                Debug.Log("ResetSound");
            }
            else
            {
                yield return null;
            }
        }
        
    }
}
