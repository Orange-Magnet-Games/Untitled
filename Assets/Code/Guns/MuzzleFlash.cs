using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
  public float timeShown;
  private float timer;
  private void OnEnable() {
    timer = timeShown;
  }

  private void Update() {
    timer -= Time.deltaTime;
    if(timer <= 0) gameObject.SetActive(false);
  }
}
