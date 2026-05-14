using System.Collections;
using UnityEngine;


public class lightFlashing : MonoBehaviour
{

    public float flashInterval = 1f;
    public bool enableMicroFlash = false;
    public bool randomFlashes = false;
    public float microFlashInterval = 0.2f;
    public int flashQuantity = 1;
    public Light lightSource;

    int microFlashCount;

    float randomFlashInterval;

    void Start()
    {
       
        StartCoroutine(ToggleLight());
        
    }


    IEnumerator ToggleLight()
    {
        while (true)
        {
            if (randomFlashes)
            {
                randomFlashInterval = Random.Range(5, flashInterval);
                yield return new WaitForSeconds(randomFlashInterval);
            }
            else
            {
                yield return new WaitForSeconds(flashInterval);
            }

            if (randomFlashes)
            {
                StartCoroutine(FlashLight());
            }
            else if (lightSource.enabled)
            {
                lightSource.enabled = false;

            }
            else if( enableMicroFlash && flashQuantity > 1)
            {
                StartCoroutine(FlashLight());
            }
            else
            {
                lightSource.enabled = true;
            }
            
        }


    }

    IEnumerator FlashLight()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(microFlashInterval);
            
               



            if (microFlashCount <= (flashQuantity * 2) - 1)
            {
                microFlashCount = microFlashCount + 1;

                if (lightSource.enabled)
                {
                    lightSource.enabled = false;
                }
                else
                {
                    lightSource.enabled = true;
                }
            }
            else
            {
                microFlashCount = 0;

                if (randomFlashes)
                {
                    lightSource.enabled = true;
                }

                yield break;
            }
        }
    }

}
