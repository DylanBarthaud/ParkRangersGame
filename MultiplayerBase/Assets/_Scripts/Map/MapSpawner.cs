using System.Collections.Generic;
using UnityEngine;

public class MapSpawner
{
    private List<GameObject> spawns = new List<GameObject>();
    private float maxSteepness; 

    public MapSpawner(float maxSteepness)
    {
        this.maxSteepness = maxSteepness;
    }

    public (int[] objIds, Vector3[] objPositions) Spawn(Terrain terrain, GameObject[] spawnableObjects, int numberOfObjects)
    {
        List<int> objectIds = new List<int>();
        List<Vector3> objPositions = new List<Vector3>();

        for (int i = 0; i < numberOfObjects; i++) 
        {
            int objectIndex = Random.Range(0, spawnableObjects.Length);
            bool objectSpawned = false;
            Vector3 spawnLoc = new Vector3(); 
            (objectSpawned, spawnLoc) = SpawnObject(terrain, spawnableObjects[objectIndex]);
        }

        return (objectIds.ToArray(), objPositions.ToArray());
    }

    private (bool, Vector3) SpawnObject(Terrain terrain, GameObject spawnObject)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        const int maxAttempts = 5;  
        for (int i = 0; i < maxAttempts; i++)
        {
            float xPos = Random.Range(0f, terrainSize.x);
            float zPos = Random.Range(0f, terrainSize.z);
            float yPos = terrain.SampleHeight(new Vector3(xPos, 0f, zPos));

            float normX = xPos / terrainSize.x;
            float normZ = zPos / terrainSize.z;

            if (terrainData.GetSteepness(normX, normZ) <= maxSteepness)
            {
                Vector3 spawnPoint = new Vector3(xPos, yPos, zPos);
                GameManager.instance.SpawnObjectOnNetwork(
                    spawnObject,
                    spawnPoint,
                    Quaternion.identity
                );
                spawns.Add(spawnObject);
                return (true, spawnPoint); 
            }
        }

        return (false, Vector3.zero);   
    }

    public void SpawnObjectsAtLoc(GameObject[] spawnObjects, Vector3[] spawnObjPositions)
    {
        int i = 0; 
        foreach (var spawnObject in spawnObjects)
        {
            GameManager.instance.SpawnObjectOnNetwork(
                spawnObject,
                spawnObjPositions[i],
                Quaternion.identity
            );

            i++;
        }
    }
}
