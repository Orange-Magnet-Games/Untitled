using UnityEngine;

public class Projectile : MonoBehaviour {
  [HideInInspector] public Vector3 forward;
  [HideInInspector] public float speed;
  [HideInInspector] public int damage;
  [SerializeField] private float lifeTimer;
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      other.GetComponent<PlayerStats>().TakeDamage(damage);
      Destroy(gameObject);
    }
    else if (other.CompareTag("Ground")) {
      Destroy(gameObject);
    }
  }
  private void Update() {
    transform.position += forward * (speed * Time.deltaTime);

    lifeTimer -= Time.deltaTime;
    if(lifeTimer <= 0) Destroy(gameObject);
  }
}
