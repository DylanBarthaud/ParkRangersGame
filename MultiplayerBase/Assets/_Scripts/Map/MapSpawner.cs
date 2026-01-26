using System.Collections.Generic;
using UnityEngine;

public class MapSpawner
{
    private List<GameObject> spawns = new List<GameObject>();

    public void Spawn(Terrain terrain, GameObject[] spawnableObjects, int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++) 
        {
            int objectIndex = Random.Range(0, spawnableObjects.Length);
            SpawnObject(terrain, spawnableObjects[objectIndex]);
        }
    }

    private void SpawnObject(Terrain terrain, GameObject spawnObject)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        int xPos = Random.Range(0, (int)terrainSize.x + 1);
        int zPos = Random.Range(0, (int)terrainSize.z + 1);
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));

        Vector3 spawnPoint = new Vector3(xPos, yPos, zPos);

        GameManager.instance.SpawnObjectOnNetwork(spawnObject, spawnPoint, Quaternion.identity); 
        spawns.Add(spawnObject);
    }
}
