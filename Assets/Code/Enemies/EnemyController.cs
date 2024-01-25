using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour {
  [HideInInspector] public int health;
  [SerializeField] private int maxHealth;
  [SerializeField] private Slider healthSlider;
  [SerializeField] private float attackDistance;
  public int damage;
  [SerializeField] private Pickup pickup;
  private NavMeshAgent agent;
  private Transform player;
  [SerializeField] private Animator anim;
  private static readonly int Walking = Animator.StringToHash("Walking");
  private static readonly int Attacking = Animator.StringToHash("Attacking");
  [SerializeField] private GutController guts;
  private void Start() {
    health = maxHealth;
    healthSlider.maxValue = maxHealth;
    player = GameManager.instance.player.transform;
    //anim.GetComponentInChildren<Animator>();
    agent = GetComponent<NavMeshAgent>();
  }

  // ReSharper disable Unity.PerformanceAnalysis
  public void TakeDamage(int d) {
    health -= d;
    if (health > 0) return;
    int gunChance = Random.Range(0, 40);
    Transform tr = transform;
    if (gunChance <= 4) {
      Pickup pup = Instantiate(pickup, tr.position, Quaternion.identity);
      pup.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 5, 0);
      pup.gun = gunChance;
      Gun[] guns = GameManager.instance.gunManager.allPossibleGuns;
      pup.ammoInGun = Random.Range(guns[gunChance].maxAmmoInMag * 5, guns[gunChance].maxAmmoInMag * 15);
    }

    for (int i = 0; i < Random.Range(3, 6); i++) {
      Instantiate(guts, tr.position, tr.rotation);
    }
    Destroy(gameObject);
  }

  private void Update() {
    healthSlider.value = health;
    healthSlider.transform.parent.gameObject.SetActive(health != maxHealth);
  }

  private void FixedUpdate() {
    Vector3 playerPos = player.position;
    agent.SetDestination(playerPos);

    Vector3 rayDirection = playerPos - transform.position;
    if (Vector3.Distance(playerPos, transform.position) < attackDistance) {
      if (!Physics.Raycast(transform.position, rayDirection, out RaycastHit hit)) return;
      if (hit.transform != player) return;
      agent.isStopped = true;
      anim.SetBool(Attacking, true);
    }
    else agent.isStopped = anim.GetBool(Attacking);

    anim.SetBool(Walking, !agent.isStopped);
  }
}
