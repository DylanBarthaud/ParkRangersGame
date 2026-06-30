using System.Runtime.ExceptionServices;
using UnityEngine;

[ExecuteAlways]
public class skyboxScript : MonoBehaviour
{

    float randomNumber;
    public float spawnChance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        randomNumber = Random.Range(0f, 1f);

        if (randomNumber < spawnChance)
        {
            Shader.SetGlobalInt("_EnableAurora", 1);

            Debug.Log("Aurora Activity:" + Shader.GetGlobalInt("_EnableAurora"));
        }
        else
        {
            Shader.SetGlobalInt("_EnableAurora", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
