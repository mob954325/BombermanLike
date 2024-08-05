using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerAnimation : NetworkBehaviour
{
    Animator animator;
    PlayerMovement playerMovement;
    PlayerBehaviour playerBehaviour;

    int HashToSpeed = Animator.StringToHash("Speed");
    int HashToDie = Animator.StringToHash("Die");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerBehaviour = GetComponentInParent<PlayerBehaviour>();

        playerBehaviour.OnDie += PlayDieAnim;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        animator.SetFloat(HashToSpeed, playerMovement.moveSpeed);
    }

    private void PlayDieAnim()
    {
        animator.SetTrigger(HashToDie);
    }
}