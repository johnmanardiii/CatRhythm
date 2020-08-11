using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SyncedAnimation : MonoBehaviour
{
    public Animator animator;
    public AnimatorStateInfo animatorStateInfo;
    public int currentState;
    private Conductor conductor;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentState = animatorStateInfo.fullPathHash;
        conductor = FindObjectOfType<Conductor>();
    }

    void Update()
    {
        animator.Play(currentState, -1, (conductor.GetBeatPosition()));
        animator.speed = 0;
    }
}
