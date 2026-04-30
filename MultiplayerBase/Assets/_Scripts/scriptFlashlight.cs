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
    public Camera playerCamera;
    public AnimationCurve curve;

    
    float targetIntensity;
    float currentIntensity;

    float lerpDuration = 3;
    float startValue = 0;
    float endValue = 10;
    float valueToLerp;
    float curvePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curve = AnimationCurve.EaseInOut(maxIntensity, 15, minIntensity, 5);
    } 

    // Update is called once per frame
    void Update()
    {
        if (Physics.SphereCast(playerCamera.transform.position, 0.5f, playerCamera.transform.forward, out hit, 50))
        {
            if (hit.distance <= 1.5f)
            {
                targetIntensity = minIntensity;
            }
            else if (hit.distance <= 5)
            {
                targetIntensity = (minIntensity + 10f);
            }
            else
            {
                targetIntensity = maxIntensity;
            }

            

            light.intensity = Mathf.MoveTowards(light.intensity, targetIntensity, (lerpSpeed * curve.Evaluate(targetIntensity)) / Time.deltaTime);
        }
    }
}
