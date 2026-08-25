using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class MovementSensor : NetworkBehaviour
{
    [Header("Detection")]
    [SerializeField] float radius; 
    [SerializeField] LayerMask detectionLayers;

    [Header("GFX")]
    [SerializeField] GameObject alertLight; 

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;


    public override void OnNetworkSpawn() => EventManager.instance.onTick_5 += CheckForMovement;
    public override void OnNetworkDespawn() => EventManager.instance.onTick_5 -= CheckForMovement;

    int wait = 0;
    private void CheckForMovement(int tick)
    {
        wait++;
        if(wait < 2) return;

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
