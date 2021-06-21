using System;
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
        set
        {
            if (state == value)
                return;

            state = value;

            var animationInfo = blendingInfos.Find(x => x.state == state);
            if (animationInfo != null)
                animator.CrossFade(animationInfo.clipName, animationInfo.time);
        }
    }
    enum StateType
    {
        Idle,
        Run,
        Jump,
        Attack,
        None
    }
    Dictionary<string, float> animationLength = new Dictionary<string, float>();
    void Start()
    {
        animator = GetComponent<Animator>();
        foreach (var item in animator.runtimeAnimatorController.animationClips)
        {
            animationLength[item.name] = item.length;
        }
    }
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
                //var animationInfo = blendingInfos.Find(x => x.state == StateType;
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
    [SerializeField] float attackAnimationWaitTimeRate = 0.7f;
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
            //float attackAnimationTime = animationLength["Attack"];
            //yield return new WaitForSeconds(attackAnimationTime);
            yield return null;

            var stateinfo = animator.GetCurrentAnimatorStateInfo(0);
            var delay = stateinfo.length * attackAnimationWaitTimeRate;
            yield return new WaitForSeconds(delay);

            State = StateType.None;
        }
    }
    [SerializeField] List<BlendingInfo> blendingInfos;
    [Serializable]
    class BlendingInfo
    {
        public StateType state;
        public string clipName;
        public float time;
    }
}
