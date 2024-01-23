using UnityEngine;

public class AnimationEvents : MonoBehaviour {
  private Animator anim;
  private static readonly int Attacking = Animator.StringToHash("Attacking");
  [SerializeField] private Projectile projectile;
  private EnemyController controller;
  private void Start() {
    anim = GetComponent<Animator>();
    controller = GetComponentInParent<EnemyController>();
  }

  private void AttackOver() {
    anim.SetBool(Attacking, false);
  }

  private void FireProjectile(float speed) {
    Projectile proj = Instantiate(projectile);
    proj.damage = controller.damage;
    proj.speed = speed;
    
    Transform tr = transform;
    proj.forward = tr.forward;
    proj.transform.position = tr.position;
  }
}
