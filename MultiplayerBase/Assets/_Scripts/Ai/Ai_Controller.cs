using UnityEngine;
using UnityEngine.AI;

public class Ai_Controller : MonoBehaviour
{
    [SerializeField] private float defaultSpeed, runSpeed;
    [SerializeField] private float atkDmg;

    [SerializeField] NavMeshAgent agent;

    public void ChangeSpeed()
    {
        agent.speed = defaultSpeed;
    }

    public void MoveToLocation()
    {

    }

    public void ChasePlayer()
    {

    }
}
