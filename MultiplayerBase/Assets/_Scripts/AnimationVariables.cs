using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode.Components;

public class AnimationVariables : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    private FirstPersonController _firstPersonVariables;
    private ClientSettings clientSettings;

    private Vector3 _lastPosition = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _firstPersonVariables = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = transform.position;
        Vector3 newVector = currentPos - _lastPosition;

        _animator.SetFloat("MoveX", newVector.x);
        _animator.SetFloat("MoveZ", newVector.z);

        if (_firstPersonVariables.IsSprinting == true)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }

        if (_firstPersonVariables.IsCrouching == true)
        {
            _animator.SetBool("IsCrouching", true);
        }
        else
        {
            _animator.SetBool("IsCrouching", false);
        }
    }
}
