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

    public (int[] objIds, Vector3[] objPositions) Spawn(Terrain terrain, GameObject[] spawnableObjects, int numberOfObjects)
    {
        List<int> objectIds = new List<int>();
        List<Vector3> objPositions = new List<Vector3>();

        for (int i = 0; i < numberOfObjects; i++) 
        {
            int objectIndex = Random.Range(0, spawnableObjects.Length);
            bool objectSpawned = false;
            Vector3 spawnLoc = new Vector3(); 
            (objectSpawned, spawnLoc) = SpawnObject(terrain, objectIndex);
        }

        return (objectIds.ToArray(), objPositions.ToArray());
    }

    private (bool, Vector3) SpawnObject(Terrain terrain, int spawnObjectId)
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
                return (true, spawnPoint); 
            }
        }

        return (false, Vector3.zero);   
    }

    public void SpawnObjectsAtLoc(int[] spawnObjectIds, Vector3[] spawnObjPositions)
    {
        int i = 0; 
        foreach (var spawnObjectId in spawnObjectIds)
        {
            GameManager.instance.SpawnObjectOnNetworkServerRpc(
                spawnObjectId,
                spawnObjPositions[i],
                Quaternion.identity
            );

            i++;
        }
    }
}
