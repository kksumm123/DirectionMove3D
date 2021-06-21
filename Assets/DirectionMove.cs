﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionMove : MonoBehaviour
{
    Animator animator;

    [SerializeField] float speed = 5f;
    [SerializeField] Vector3 lastMoveDir;
    [SerializeField] float rotateLerp = 0.05f;



    [SerializeField] StateType state;
    StateType State
    {
        get { return state; }
        set { state = value; }
    }
    enum StateType
    {
        Idle,
        Run,
        Jump,
        Attack,
        None
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    Dictionary<string, float> animationLength = null;
    void Update()
    {
        Move();
        Attack();
    }


    Vector3 move;
    private void Move()
    {
        if (State != StateType.Attack)
        {
            move = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
                move.x = -1;
            if (Input.GetKey(KeyCode.D))
                move.x = 1;
            if (Input.GetKey(KeyCode.W))
                move.z = 1;
            if (Input.GetKey(KeyCode.S))
                move.z = -1;
            if (move != Vector3.zero)
            {
                State = StateType.Run;
                animator.Play("Run");
                move.Normalize();
                transform.Translate(move * speed * Time.deltaTime, Space.World);

                lastMoveDir = move;
            }
            else
            {
                State = StateType.Idle;
                animator.Play("Idle");
            }
        }

        transform.forward = Vector3.Slerp(transform.forward, lastMoveDir, rotateLerp);
    }
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 아래처럼 레이어 + 블랜더 사용하면 애니메이션이 섞이지만 
            // animator.Play("Attack", 1);
            // 이번에는 
            // 어택을 했으면 어택 애니메이션 진행중인 동안
            // Run과 Idle 안되게 하자
            // 각 애니메이션의 길이가 필요
            // 어택이 끝나면 State = None
            StartCoroutine(AttackCo());
        }
        IEnumerator AttackCo()
        {
            State = StateType.Attack;
            animator.Play("Attack");
            float attackAnimationTime = animationLength["Attack"];
            yield return new WaitForSeconds(attackAnimationTime);
            State = StateType.None;
        }
    }

}
