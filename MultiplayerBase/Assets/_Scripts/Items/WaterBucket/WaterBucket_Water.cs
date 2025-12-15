using UnityEngine;

public class WaterBucket_Water : MonoBehaviour
{
    [SerializeField] private Interactor interactor;

    private void Start()
    {
        interactor.CircleInteract();

        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null) Destroy(gameObject, particleSystem.main.startLifetime.constant);
    }
}
