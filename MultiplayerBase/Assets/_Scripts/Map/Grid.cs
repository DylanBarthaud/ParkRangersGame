using Unity.VisualScripting;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize; 

    private int[,] gridPositions;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridPositions = new int[width, height];

        for(int x = 0; x < gridPositions.GetLength(0); x++)
        {
            for(int y = 0; y < gridPositions.GetLength(1); y++)
            {
                Vector3 worldPos = GetWorldPos(x, y);
                Debug.DrawLine(worldPos, worldPos + Vector3.up * 10f, Color.red, 100f);
            }
        }
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize;
    }

    public GridPosition GetGridPos(Vector3 worldPos)
    {
        GridPosition gridPosition = new GridPosition();
        gridPosition.x = Mathf.FloorToInt(worldPos.x / cellSize);
        gridPosition.y = Mathf.FloorToInt(worldPos.z / cellSize);

        return gridPosition;  
    }

    public float GetCellSize()
    {
        return cellSize;
    }
}
