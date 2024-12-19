using UnityEngine;

public class CharacterController2D : MonoBehaviour {
    [SerializeField] private float jumpForce = 100f;                          // Amount of force added when the player jumps.
    [SerializeField] private float runSpeed = 150f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask whatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.

    private const float GroundedRadius = .3f; // Radius of the overlap circle to determine if grounded
    public bool grounded;            // Whether the player is grounded.
    private Rigidbody2D _rigidbody; // Player's rigidbody component
    private bool _facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 _velocity = Vector3.zero;

    private float _originalScaleToKeep;

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate(){ // WM_F02
        grounded = false; // The player is not grounded by default

        // Here we are checking if the player is touching ground by creating a sphere around the Ground Check position with the
        // radius of Grounded Radius, and if the sphere overlaps with an object of the What Is Ground layer, then the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, whatIsGround);
        foreach (var t in colliders)
            if (t.gameObject != gameObject)// excluding the component bearer
                grounded = true;
    }
    public void Move(float move, bool jump){ // WM_F02
        // Using the physics engine to set velocity (not to apply force!) to the player's body
        float horizontalMove = move * runSpeed * Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMove, _rigidbody.velocity.y);
        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, movementSmoothing); 
       
        // Change the way the sprite is facing if turned around
        if (horizontalMove > 0 && !_facingRight || horizontalMove < 0 && _facingRight) 
            Flip();

        if (!grounded || !jump) return; // Moving is finished unless the player jumping
        
        _rigidbody.AddForce(new Vector2(0f, jumpForce * 1.3f)); // Jumping is just throwing the body upwards with force
        grounded = false;
    }
    
    // Since the player is represented by a 2D sprite, it has to be flipped (mirrored) when facing left or right.
    // This is done by rotating the sprite 180 degrees around the Y axis.
    private void Flip(){ // WM_F02
        _facingRight = !_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}