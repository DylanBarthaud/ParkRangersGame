using Unity.Netcode;
using UnityEngine;

public class ObjectPools : MonoBehaviour
{
    public static ObjectPools Instance;
    public BatteryPool BatteryPool = new();

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Battery SpawnBattery(GameObject obj)
    {
        GameObject newObj = Instantiate(obj, transform);
        newObj.GetComponent<NetworkObject>().Spawn();
        return newObj.GetComponent<Battery>();
    }
}
