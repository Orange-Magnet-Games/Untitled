using UnityEngine;

public class GroundCheck : MonoBehaviour {
  private PlayerController player;

  private void Start() {
    player = GetComponentInParent<PlayerController>();
  }

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Ground")) player.isGrounded = true;
  }

  private void OnTriggerExit(Collider other) {
    if (other.CompareTag("Ground")) player.isGrounded = false;
  }
}