using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] SirenSpin sirenController;
    [SerializeField] AudioSource sirenSource; 

    [Header("Tower Settings")]
    [SerializeField] private int maxPower;

    private int currentPower = 0;

    public void EnableFloodLights() => sirenController.StartSpin(!sirenController.IsSpinning);
    public void EnableSiren()
    {
        if(sirenSource.isPlaying) sirenSource.Stop();
        else sirenSource.Play();
    }
}
