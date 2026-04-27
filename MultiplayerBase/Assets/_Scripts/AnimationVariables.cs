using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationVariables : MonoBehaviour
{

    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    private FirstPersonController _firstPersonVariables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _firstPersonVariables = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Animator.SetFloat("MoveX", _firstPersonVariables.targetVelocity);
        

    }
}
