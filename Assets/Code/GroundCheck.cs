using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
  private PlayerController player;

  private void Start() {
    player = GetComponentInParent<PlayerController>();
  }

  private void OnTriggerEnter(Collider other) {
    if (!other.CompareTag("Ground")) return;
    
    player.isGrounded = true;
    player.rb.velocity = Vector3.zero;
    
  }

  private void OnTriggerExit(Collider other) {
    if (other.CompareTag("Ground")) player.isGrounded = false;
  }

  private void OnTriggerStay(Collider other) {
    if (player.isGrounded) return;
    if (!other.CompareTag("Ground")) return;
    
    player.isGrounded = true;
    player.rb.velocity = Vector3.zero;
  }
}