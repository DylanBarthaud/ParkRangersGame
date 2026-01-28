using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class NetworkPrefabRegistry : MonoBehaviour
{
    public static NetworkPrefabRegistry Instance;

    [SerializeField] private List<GameObject> networkPrefabs;
    private Dictionary<int, GameObject> lookup;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

            lookup = new Dictionary<int, GameObject>();
        for (int i = 0; i < networkPrefabs.Count; i++)
        {
            lookup.Add(i, networkPrefabs[i]);
        }
    }

    public GameObject GetPrefab(int id)
    {
        return lookup[id];
    }
}
