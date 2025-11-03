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
}