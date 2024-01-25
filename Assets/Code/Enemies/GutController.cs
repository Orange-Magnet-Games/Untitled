using UnityEngine;

public class GutController : MonoBehaviour {
  [SerializeField] private bool spawnMore;
  [SerializeField] private GutController extraGuts;
  [SerializeField] private BulletHole splat;
  private float timer;

  private void Start() {
    Rigidbody rb = GetComponent<Rigidbody>();
    rb.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(20f, 45f);
    rb.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(10f, 15f);
  }

  private void OnCollisionEnter(Collision other) {
    Destroy(gameObject);
    if (!Physics.Raycast(transform.position, other.contacts[0].point - transform.position, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("LevelGeom"))) return;
    BulletHole sp = Instantiate(splat, hit.point + hit.normal * .001f, Quaternion.FromToRotation(Vector3.up, hit.normal));
  }

  private void Update() {
    timer -= Time.deltaTime;
    if (!spawnMore || timer > 0) return;
    
    GutController gut = Instantiate(extraGuts);
    gut.spawnMore = false;
    timer = 0.5f;
  }
}
