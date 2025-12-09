using Unity.Netcode;
using UnityEngine;

public class Campfire : NetworkBehaviour, IInteractable
{
    [SerializeField] MeshRenderer logs; 
    [SerializeField] Material putOutMat;

    [SerializeField] ParticleSystem ps;

    [SerializeField] AudioSource audioSource;

    [SerializeField] Light pointLight; 
    private bool putOut = false;

    public bool CanInteract(Interactor interactor)
    {
        if (interactor.gameObject.CompareTag("Water") && !putOut)
        {
            return true;
        }
        return false;
    }

    public void OnInteract(Interactor interactor)
    {
        EventManager.instance.OnButtonPressed();
        logs.material = putOutMat;
        ps.Stop();
        audioSource.Stop();
        pointLight.enabled = false;
        putOut = true;
    }
}
