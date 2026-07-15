using UnityEngine;

public class TrailCamLightAutoEnable : MonoBehaviour
{
    public GameObject lightSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightSource.SetActive(true);
    }

    
}
