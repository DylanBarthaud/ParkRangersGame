using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimationVariables : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator animator;

    void Update()
    {
        //Debug.Log(_agent.speed); 
        animator.SetFloat("WalkSpeed", _agent.speed);
    }
}
