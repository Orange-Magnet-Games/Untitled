using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {
    private Animator anim;
    private static readonly int Attacking = Animator.StringToHash("Attacking");

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void AttackOver() {
        anim.SetBool(Attacking, false);
    }
}
