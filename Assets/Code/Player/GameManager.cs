using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
  public static GameManager instance;
  private void OnEnable() {
    if (instance) {
      Destroy(gameObject);
    }
    else {
      instance = this;
    }
  }

  public PlayerController player;
  public Camera mainCamera;
  public Camera portalCamera;
  public GunController gunManager;
  public UIManager uiManager;
  public Volume postProcess;
  public PauseMenu pauseMenu;
  public SoundManager soundManager;
}