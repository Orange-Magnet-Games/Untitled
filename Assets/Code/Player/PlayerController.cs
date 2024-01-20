using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  public InputMaster input;
  private Vector2 direction;
  [SerializeField] private float speed, jumpPower, drag, airDrag;
  private bool jumping;

  public bool isGrounded;

  [HideInInspector] public Rigidbody rb;
  private Camera cam;

  private void OnEnable() {
    input = new InputMaster();
    input.Enable();
  }

  private void OnDisable() => input.Disable();

  private void Start() {
    
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    cam = GameManager.instance.mainCamera;
    rb = GetComponent<Rigidbody>();

    CameraLook camLook = cam.GetComponent<CameraLook>();

    input.Camera.Look.performed += ctx => camLook.OnLook(ctx, true);
    input.Camera.Look.canceled += ctx => camLook.OnLook(ctx, false);

    input.Movement.Direction.performed += ctx => OnDirection(ctx, true);
    input.Movement.Direction.canceled += ctx => OnDirection(ctx, false);

    input.Movement.Jump.performed += _ => jumping = true;
    input.Movement.Jump.canceled += _ => jumping = false;
  }

  private void OnDirection(InputAction.CallbackContext ctx, bool performed) {
    direction = performed ? ctx.ReadValue<Vector2>() : Vector2.zero;
  }

  private void FixedUpdate() {
    Vector3 vel = rb.velocity;
    
    Move(ref vel);
    Jump(ref vel);
    
    
    Drag(ref vel, 1 - (isGrounded ? drag : airDrag));

    rb.velocity = vel;
    rb.angularVelocity = Vector3.zero;
    transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);
  }

  private void Move(ref Vector3 vel) {
    
    gameObject.transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);
    
    if (!(direction.magnitude >= .1f)) return;
    
    Transform objTransform = gameObject.transform;

    vel += (objTransform.right * direction.x + objTransform.forward * direction.y) * speed;
  }

  private void Jump(ref Vector3 vel) {
    if (!jumping || !isGrounded) return;

    jumping = false;
    isGrounded = false;

    Transform objTransform = gameObject.transform;
    
    vel = new Vector3(vel.x, jumpPower, vel.z) + (objTransform.right * direction.x + objTransform.forward * direction.y) * (jumpPower * speed);
  }

  #region Drag

  // ReSharper disable once UnusedMember.Local
  private static Vector3 Drag(Vector3 vec, float drag) {
    float y = vec.y;
    vec *= drag;
    vec.y = y;
    return vec;
  }

  private static void Drag(ref Vector3 vec, float drag) {
    float y = vec.y;
    vec *= drag;
    vec.y = y;
  }

  #endregion
}