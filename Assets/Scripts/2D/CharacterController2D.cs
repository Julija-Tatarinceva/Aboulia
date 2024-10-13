using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour {
    [SerializeField] private float jumpForce = 100f;                          // Amount of force added when the player jumps.
    [SerializeField] private float runSpeed = 150f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask whatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.

    const float groundedRadius = .3f; // Radius of the overlap circle to determine if grounded
    public bool grounded;            // Whether the player is grounded.
    private Rigidbody2D rigidbody; // Player's rigidbody component
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;
    private PlayerMovement2D movementScript;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    float originalScaleToKeep;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake(){
        rigidbody = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovement2D>();
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }
    private void FixedUpdate(){
        bool wasGrounded = grounded; // Saving the old value to check later if it has changed between frame updates
        grounded = false; // The player is not grounded by default

        // Here we are checking if the player is touching ground by creating a sphere around the Ground Check
        // position with the radius of Grounded Radius, and if the sphere overlaps with an object of the What Is Ground
        // layer, then the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        foreach (var t in colliders){
            if (t.gameObject != gameObject){ // excluding the component bearer
                grounded = true;
                // If the player has been airborne the previous frame, then we need to invoke an event for landing.
                // It can, for example, play a special landing sound or break a jumping animation early.
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }
    public void Move(float move, bool jump){
        // Using the physics engine to set velocity (not to apply force!) to the player's body
        float _horizontalMove = move * runSpeed * Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(_horizontalMove, rigidbody.velocity.y);
        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing); 
       
        // Change the way the sprite is facing if turned around
        if (_horizontalMove > 0 && !facingRight || _horizontalMove < 0 && facingRight) 
            Flip();

        if (!grounded || !jump) return; // Moving is finished unless the player jumping
        
        rigidbody.AddForce(new Vector2(0f, jumpForce * 1.3f)); // Jumping is just throwing the body upwards with force
        grounded = false;
    }
    
    // Since the player is represented by a 2D sprite, it has to be flipped (mirrored) when facing left or right.
    // This is done by rotating the sprite 180 degrees around the Y axis.
    private void Flip(){
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}