using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PortalTraveller {
    public InputMaster input;
    private Vector2 direction;
    [SerializeField] private float speed, jumpPower, sprintSpeed, drag;
    private bool sprinting, jumping;

    public bool isGrounded;
    
    [HideInInspector] public Rigidbody rb;
    private Camera cam;

    private void OnEnable() {
        input = new InputMaster();  
        input.Enable();
    }
    private void OnDisable() { 
        input.Disable();
    }

    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = Manager.instance.mainCamera;
        rb = GetComponent<Rigidbody>();

        input.Movement.Direction.performed += ctx => OnDirection(ctx, true);
        input.Movement.Direction.canceled += ctx => OnDirection(ctx, false);
        
        input.Movement.Sprint.performed += _ => sprinting = true;
        input.Movement.Sprint.canceled += _ => sprinting = false;

        input.Movement.Jump.performed += _ => jumping = true;
        input.Movement.Jump.canceled += _ => jumping = false;
    }

    private void OnDirection(InputAction.CallbackContext ctx, bool performed) {
        direction = performed ? ctx.ReadValue<Vector2>() : Vector2.zero;
    }
    private void Update() {
        Move();
        Jump();
    }

    private void Move() {
        gameObject.transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);

        Vector3 vel = rb.velocity;
        
        if (direction.magnitude >= .1f) {
            
            Transform objTransform = gameObject.transform;
            
            vel += (sprinting ? sprintSpeed : speed) * (objTransform.right * direction.x + objTransform.forward * direction.y);
            
        }

        Drag(ref vel, 1 - drag - (sprinting ? .1f : 0f));
        
        rb.velocity = vel;
    }

    private void Jump() {
        if (!jumping || !isGrounded) return;
        
        jumping = false; isGrounded = false;

        Vector3 velocity = rb.velocity;
        velocity = new Vector3(velocity.x, jumpPower, velocity.z);
        rb.velocity = velocity;
    }
#region Drag
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
