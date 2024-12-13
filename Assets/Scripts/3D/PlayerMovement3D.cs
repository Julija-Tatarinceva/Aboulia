using UnityEngine;

public class PlayerMovement3D : MonoBehaviour {
    private static readonly int Speed = Animator.StringToHash("Speed");
    public CharacterController characterController;
    public float speed = 8.0f;
    public float turnSmoothTime = 0.1f;
    public float pushForce = 5.0f;
    public Animator animator;
    private float _turnSmoothVelocity;
    private float _targetAngle;
    private float _verticalSpeed;
    private Vector3 _moveDirection;
    public Transform cam;
    public GameObject slicingPlanePreview;
    public SoundPlayer soundPlayer;

    // Update is called once per frame
    private void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector3(horizontal, 0f, vertical).normalized; // Turning the inputs into a normalized vector
        if (_moveDirection.magnitude >= 0.1f) { // We can proceed if the vector is not a zero vector
            _targetAngle = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // This angle is used to figure out which way the player wants to go relative to the camera rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime); // Calculating how much the current rotation has to be changed to reach the new one
            transform.rotation = Quaternion.Euler(0f, angle, 0f); // Turning the player's body in the direction it's moving
            
            // The vector of movement is now calculated by rotating a forward vector the same way the player has just been rotated
            Vector3 direction = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward; 
            // The controller component handles the movement of the body
            characterController.Move(direction.normalized * (speed * Time.deltaTime));
        }
        if (Input.GetKeyDown("q")) {
            slicingPlanePreview.SetActive(true);
        }
        if (Input.GetKeyUp("q")) {
            slicingPlanePreview.SetActive(false);
        }
        animator.SetFloat(Speed, Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        soundPlayer.PlayStepSound(Mathf.Abs(horizontal) > 0, characterController.isGrounded);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic) {
            body.velocity = hit.moveDirection.normalized * pushForce;
        }
    }
}
