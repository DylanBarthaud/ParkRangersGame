using System.Collections;
using UnityEngine;

public class RavenController : MonoBehaviour
{
    [SerializeField] GameObject ravenObj;
    [SerializeField] PlayerInfoHolder playerInfoHolder;

    [Header("Raven Stats")]
    [SerializeField] int spawnInterval;
    [SerializeField, Range(0,100)] int spawnChance;
    [SerializeField] float spawnRadius; 

    private void Awake()
    {
        EventManager.instance.onTick += OnTick;
    }

    int localTick = 0; 
    private void OnTick(int tick)
    {
        localTick++;
        if(localTick <  spawnInterval)
        {
            return;
        }

        for(int i = 0; i < playerInfoHolder.GetPlayerInfo().ravenCount; i++)
        {
            int roll = Random.Range(0, 101);
            if (spawnChance >= roll) StartCoroutine(spawnRaven(Random.Range(0, (spawnInterval / 5) - 0.1f))); 
            localTick = 0; 
        }
    }

    public IEnumerator spawnRaven(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(ravenObj, GetPointOnRing(), Quaternion.identity);
    }

    public Vector3 GetPointOnRing()
    {
        Vector3 point = new Vector3();

        float angle = Random.Range(0f, Mathf.PI * 2f);
        point = playerInfoHolder.GetPlayerInfo().position + new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            0f,
            Mathf.Sin(angle) * spawnRadius
        );

        return point; 
    }
}
