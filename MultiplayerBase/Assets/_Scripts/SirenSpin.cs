using System.Collections;
using UnityEngine;

public class SirenSpin : MonoBehaviour
{
    public float speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Spin());
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Rotate(0f, speed, 0f, Space.World);
    }


    IEnumerator Spin()
    {
        float i = 0;
        while (i < 1)
        {
            gameObject.transform.Rotate(0f, speed, 0f, Space.World);
            yield return null;
        }

        yield return new WaitForSeconds(.1f);
    }

}
