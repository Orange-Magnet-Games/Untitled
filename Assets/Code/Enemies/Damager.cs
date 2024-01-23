using UnityEngine;

// ReSharper disable once IdentifierTypo
public class Damager : MonoBehaviour {
  [SerializeField] private int damage;
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) other.GetComponent<PlayerStats>().TakeDamage(damage);
  }
}
 