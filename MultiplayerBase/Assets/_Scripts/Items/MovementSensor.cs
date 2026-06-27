using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MovementSensor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] float radius; 
    [SerializeField] LayerMask detectionLayers;

    [Header("GFX")]
    [SerializeField] GameObject alertLight; 

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;


    private void Awake()
    {
        EventManager.instance.onTick_5 += CheckForMovement;
    }

    int wait = 0;
    private void CheckForMovement(int tick)
    {
        wait++;
        if(wait < 2) return;

        Debug.Log("HERE"); 
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, detectionLayers);

        if (colliders.Length <= 0) return;
        else
        {
            audioSource.Play();
            StartCoroutine(FlashLight()); 
        }

        wait = 0;
    }

    private IEnumerator FlashLight()
    {
        alertLight.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        alertLight.SetActive(false);
    }
}
