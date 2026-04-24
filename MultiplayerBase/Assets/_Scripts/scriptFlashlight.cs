using BlackboardSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class scriptFlashlight : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    float distance;
    
    public Light light;
    public float lerpSpeed = 1f;
    public float maxIntensity;
    public float minIntensity;
    float targetIntensity;
    float currentIntensity;

    float lerpDuration = 3;
    float startValue = 0;
    float endValue = 10;
    float valueToLerp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 50))
        {
            if (hit.distance <= 1)
            {
                targetIntensity = minIntensity;
            }
            else if (hit.distance < 5)
            {
                targetIntensity = 10;
            }
            else
            {
                targetIntensity = maxIntensity;
            }

            currentIntensity = light.intensity;

            light.intensity = Mathf.MoveTowards(currentIntensity, targetIntensity, lerpSpeed / Time.deltaTime);
        }
    }
}
