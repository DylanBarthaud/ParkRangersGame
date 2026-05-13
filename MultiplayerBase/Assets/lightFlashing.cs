using System.Collections;
using UnityEngine;


public class lightFlashing : MonoBehaviour
{

    public float flashInterval = 1;
    public Light lightSource;

    void Start()
    {
        StartCoroutine(ToggleLight());

    }


    IEnumerator ToggleLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(flashInterval);
            Debug.Log("Looping");

            if (lightSource.enabled)
            {
                lightSource.enabled = false;
            }
            else
            {
                lightSource.enabled = true;
            }

           
        }

    }

}
