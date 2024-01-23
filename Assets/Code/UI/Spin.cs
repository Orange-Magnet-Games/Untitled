using UnityEngine;

public class Spin : MonoBehaviour {
  [SerializeField] private Vector3 spin;
  [SerializeField] private float speed;
  private Vector3 spinnies, target;
  [SerializeField] private bool randomized;
  private float timer;


  private void FixedUpdate() {
    if (randomized) {
      spinnies = Vector3.Lerp(spinnies, target, 0.1f);

      if (timer > 0) timer -= Time.deltaTime;
      else { 
        target = new Vector3(Random.Range(-spin.x, spin.x), Random.Range(-spin.y, spin.y), Random.Range(-spin.z, spin.z)) * speed;
        timer = Random.Range(1f, 3f);
      }
    }
    else spinnies = spin;

    transform.Rotate(spinnies.normalized, spinnies.magnitude, Space.Self);
  }
}
