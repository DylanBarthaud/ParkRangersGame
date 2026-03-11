using System.Collections.Generic;
using UnityEngine;

public class MapSpawner
{
    private List<int> spawns = new List<int>();
    private float maxSteepness; 

    public MapSpawner(float maxSteepness)
    {
        this.maxSteepness = maxSteepness;
    }

    public void Spawn(Terrain terrain, GameObject[] spawnableObjects, int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++) 
        {
            int objectIndex = Random.Range(0, spawnableObjects.Length);
            SpawnObject(terrain, objectIndex);
        }
    }

    private void SpawnObject(Terrain terrain, int spawnObjectId)
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
                GameManager.instance.SpawnObjectOnNetworkServerRpc(
                    spawnObjectId,
                    spawnPoint,
                    Quaternion.identity
                );
                spawns.Add(spawnObjectId);
                break; 
            }
        }
    }
}
