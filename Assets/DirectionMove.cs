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
    void Start()
    {
        animator = GetComponent<Animator>();

    }
    Vector3 move;
    void Update()
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
        transform.forward = Vector3.Slerp(transform.forward, lastMoveDir, rotateLerp);
    }

    enum StateType
    {
        Idle,
        Run,
        Jump,
        Attack
    }
}
