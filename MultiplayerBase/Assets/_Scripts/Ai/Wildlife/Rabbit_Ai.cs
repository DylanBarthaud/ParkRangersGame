using BehaviourTrees;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Rabbit_Ai : NetworkBehaviour
{
    private NavMeshAgent agent;

    Root root;

    [Header("Base Settings")]
    [SerializeField] private float baseSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        root = new Root("Root");
        Leaf searchCell = new Leaf("SearchCell", new SearchCellStrategy(() => GameManager.instance.mapHandler.GetGridLocation(transform.position), agent));

        root.AddChild(searchCell);
    }

    private void Update()
    {
        root.Process(); 
    }
}
