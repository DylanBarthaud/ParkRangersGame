using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationVariables : MonoBehaviour
{

    NavMeshAgent _agent;
    Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<FirstPersonController>();

        //walking anims


        if (Input.GetKey(KeyCode.A))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 0.3333333f);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 0.1666666f);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 1.1666666f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 0.6666667f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 1);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            _animator.SetBool("isWalking", true);
            _animator.SetFloat("Blend", 0);
        }
        else
        {
            _animator.SetBool("isWalking", false);

        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("Blend", 0f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.A)))
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("Blend", 0.3333333f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.S)))
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("Blend", 0.6666667f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.D)))
        {
            _animator.SetBool("isRunning", true);
            _animator.SetFloat("Blend", 1f);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _animator.SetBool("isJump", true);
        }
        else
        {
            _animator.SetBool("isJump", false);
        }
    }
}
