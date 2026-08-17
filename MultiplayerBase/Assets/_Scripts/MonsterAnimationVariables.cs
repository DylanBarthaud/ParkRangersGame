using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimationVariables : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator animator;

    void Update()
    {
        if(_agent.isStopped) animator.SetFloat("WalkSpeed", 0);
        else animator.SetFloat("WalkSpeed", _agent.speed);
    }
}
