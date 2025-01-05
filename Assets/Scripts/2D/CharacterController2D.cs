using UnityEngine;

namespace _2D
{
    public class CharacterController2D : MonoBehaviour {
        [SerializeField] private float jumpForce = 100f;                          // Amount of force added when the player jumps.
        [SerializeField] private float runSpeed = 150f;
        [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement
        [SerializeField] public LayerMask whatIsGround;                          // A mask determining what is ground to the character
        [SerializeField] public Transform groundCheck;                           // A position marking where to check if the player is grounded.

        public float groundedRadius = .3f; // Radius of the overlap circle to determine if grounded
        private Rigidbody2D _rigidbody; // Player's rigidbody component
        private bool _facingRight = true;  // For determining which way the player is currently facing.
        private Vector3 _velocity = Vector3.zero;

        private float _originalScaleToKeep;

        private void Awake(){
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Move(float move, bool jump, ref bool grounded){ // WM_F02
            // Using the physics engine to set velocity (not to apply force!) to the player's body
            float horizontalMove = move * runSpeed * Time.fixedDeltaTime;
            Vector3 targetVelocity = new Vector2(horizontalMove, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, movementSmoothing); 
       
            // Change the way the sprite is facing if turned around
            if (horizontalMove > 0 && !_facingRight || horizontalMove < 0 && _facingRight) 
                Flip();

            if (!grounded || !jump) 
                return; // Moving is finished unless the player jumping
        
            _rigidbody.AddForce(new Vector2(0f, jumpForce * 1.3f)); // Jumping is throwing the body upwards with force
            grounded = false;
        }
    
        // Since the player is represented by a 2D sprite, it has to be flipped (mirrored) when facing left or right.
        private void Flip(){ // WM_F02
            _facingRight = !_facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}