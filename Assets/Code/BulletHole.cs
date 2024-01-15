using UnityEngine;

public class BulletHole : MonoBehaviour {
  [SerializeField] private float max;
  private float timer;
  private Vector3 originalScale;

  private void Start() {
    originalScale = transform.localScale;
    timer = max;
  }

  private void Update() {
    timer -= Time.deltaTime;
    transform.localScale = originalScale * (timer / max);
    
    if(timer <= 0) Destroy(gameObject);
  }
}
