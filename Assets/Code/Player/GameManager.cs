using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
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
  public Camera portalCamera;
  public GunController gunManager;
  public UIManager uiManager;
  public Volume postProcess;
  public PauseMenu pauseMenu;
}