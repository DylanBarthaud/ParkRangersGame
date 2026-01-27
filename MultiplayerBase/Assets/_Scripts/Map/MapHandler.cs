using UnityEngine;

public class MapHandler
{
    Grid mapGrid;
    int width;
    int height;
    float cellSize;

    MapSpawner spawner;
    float maxSteepness; 

    public MapHandler(int width, int height, float cellSize, Terrain terrain, float maxSteepness, GameObject[] spawnables = null, int numberOfObject = 0)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        mapGrid = new Grid(width, height, cellSize);
        spawner = new MapSpawner(maxSteepness); 
        spawner.Spawn(terrain, spawnables, numberOfObject);
    }

    public GridPosition GetGridLocation(Vector3 position)
    {
        return mapGrid.GetGridPos(position);
    }

    public Vector3 GetRandomLocationInGridPosition(GridPosition gridPosition)
    {
        float minX = gridPosition.x * cellSize;
        float minZ = gridPosition.z * cellSize;

        float maxX = minX + cellSize;
        float maxZ = minZ + cellSize;

        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="positionA"></param>
    /// <param name="positionB"></param>
    /// <returns> Squared distance between grids </returns>
    public float GetDistanceBetweenGrids(GridPosition positionA, GridPosition positionB)
    {
        return mapGrid.GetSquaredDistanceBetweenGridPositions(positionA, positionB);
    }
}