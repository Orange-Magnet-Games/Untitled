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
        int i = Random.Range(0, 40);
        if (i <= 4) {
            Pickup pup = Instantiate(pickup, transform.position, Quaternion.identity);
            pup.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 5, 0);
            pup.gun = i;
            Gun[] guns = GameManager.instance.gunManager.allPossibleGuns;
            pup.ammoInGun = Random.Range(guns[i].maxAmmoInMag * 5, guns[i].maxAmmoInMag * 15);
        }
        Destroy(gameObject);
    }

    private void Update() {
        healthSlider.value = health;
        healthSlider.transform.parent.gameObject.SetActive(health != maxHealth);
    }

    private void FixedUpdate() {
        agent.SetDestination(player.position);
        
        if (Vector3.Distance(player.position, transform.position) < attackDistance) {
            agent.isStopped = true;
            anim.SetBool(Attacking, true);
        }
        else agent.isStopped = anim.GetBool(Attacking);
        
        anim.SetBool(Walking, !agent.isStopped);
    }
}
