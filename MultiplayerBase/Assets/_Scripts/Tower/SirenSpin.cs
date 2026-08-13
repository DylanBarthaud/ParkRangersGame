using System.Collections;
using UnityEngine;

public class SirenSpin : MonoBehaviour
{
    public float speed = 1;
    [SerializeField] private GameObject lights;
    [SerializeField] private bool SpinOnStart; 

    private bool isSpinning = false;
    public bool IsSpinning => isSpinning;

    private Coroutine spinCorotine;

    private void Awake()
    {
        if(SpinOnStart) StartSpin(true);
    }

    public void StartSpin(bool enable)
    {
        isSpinning = enable;
        lights.SetActive(enable);

        if(enable) spinCorotine = StartCoroutine(Spin());
        else StopCoroutine(spinCorotine);
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
