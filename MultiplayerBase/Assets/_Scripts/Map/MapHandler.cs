using UnityEngine;

public class MapHandler : MonoBehaviour
{
    public static MapHandler instance;

    Grid mapGrid;
    [SerializeField] int width, height;
    [SerializeField] float cellSize; 

    private void Awake()
    {
        if (instance == null) instance = this;

        mapGrid = new Grid(width, height, cellSize);
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