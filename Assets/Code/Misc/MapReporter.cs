using UnityEngine;

public class MapReporter : MonoBehaviour {
  [HideInInspector] public bool playerWalkedIn;
  public EnemyWaveController enemyWaves;
  [SerializeField] private Animator door;
  private static readonly int Closed = Animator.StringToHash("Closed");
  private void OnTriggerEnter(Collider other) {
    if (!other.CompareTag("Player")) return;
    if (!playerWalkedIn) door.SetBool(Closed, true);
    playerWalkedIn = true;
  }
}
