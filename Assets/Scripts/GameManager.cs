using UnityEngine;

public class GameManager : MonoBehaviour {
  // ReSharper disable once MemberCanBePrivate.Global Will use eventually stfu
  public static GameManager instance;

  private void Awake() {
    if (instance) {
      Destroy(gameObject);
    }
    else {
      instance = this;
      DontDestroyOnLoad(instance);
    }
  }


  public PlayerController player;
  public Camera mainCamera;

  private void Start() {
    mainCamera = Camera.main;
    player = GetComponentInChildren<PlayerController>();
  }
}