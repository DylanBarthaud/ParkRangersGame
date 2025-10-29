using UnityEngine;

public class MapHandler : MonoBehaviour
{
    Grid mapGrid;
    [SerializeField] int width, height;
    [SerializeField] float cellSize; 

    private void Awake()
    {
        mapGrid = new Grid(width, height, cellSize);
    }
}
