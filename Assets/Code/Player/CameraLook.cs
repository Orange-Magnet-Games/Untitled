using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour {
  
  private PlayerController player;
  [HideInInspector] public Vector2 input;
  [SerializeField] private float offset;
  [Range(0, 1)] public float sensitivity = 1.0f;
  private PauseMenu pause;

  private void Start() {
    player = GameManager.instance.player;

    Camera cam = GetComponent<Camera>();
    RenderTexture targetTexture = cam.targetTexture;
    targetTexture.width = Screen.width;
    targetTexture.height = Screen.height;
    pause = GameManager.instance.pauseMenu;
  }

  public void OnLook(InputAction.CallbackContext ctx, bool performed) {
    input = performed ? ctx.ReadValue<Vector2>() : Vector2.zero;
  }

  public void Update() {
    if (pause.paused) return;
    Vector3 eulerAngles = transform.eulerAngles;

    eulerAngles -= new Vector3(input.y * sensitivity, -input.x * sensitivity, 0);
    eulerAngles = new Vector3(ClampAngle(eulerAngles.x, -90, 90), eulerAngles.y, eulerAngles.z);

    Transform tr = transform;
    tr.eulerAngles = eulerAngles;
    tr.position = player.transform.position + Vector3.up * offset;
  }

  public static float ClampAngle(float angle, float min, float max) {
    
    float start = (min + max) * 0.5f - 180;
    float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
    return Mathf.Clamp(angle, min + floor, max + floor);
  }
}