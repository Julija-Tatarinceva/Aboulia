using General;
using UnityEngine;

namespace _2D
{
    public class PlayerMovement2D : MonoBehaviour {
        // For more effectiveness using hashes instead of strings
        private static readonly int VerticalMove = Animator.StringToHash("VerticalMove");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
    
        public CharacterController2D characterController2D;
        private DimensionSwitcher _dimensionSwitcher;
        public LevelManager levelManager;
        private float _horizontalMove = 0f;
        public bool jump;
        public bool grounded; // Whether the player is grounded.
        public bool isRespawning;
        public Animator animator;
        public Rigidbody2D playerRigidbody2D;
        public GameObject player, spawn;
        public SoundPlayer soundPlayer;
    
        private void Update(){ // WM_F01
            // Using keys as input, these keys can be changed in project's settings
            _horizontalMove = Input.GetAxisRaw("Horizontal");
            animator.SetFloat(Speed, Mathf.Abs(_horizontalMove));
            animator.SetFloat(VerticalMove, playerRigidbody2D.velocity.y);
            animator.SetBool(IsGrounded, grounded);
            if (Input.GetButtonDown("Jump") && !jump){
                jump = true;
            }
            if(grounded && Mathf.Abs(_horizontalMove) > 0){ 
                soundPlayer.PlayStepSound();
            }
        }
        // Since movement is a physics operation, it should be done through Fixed Update, which is not tied to frame rate
        // This helps in avoiding collision and speed calculation mistakes
        private void FixedUpdate(){ 
            grounded = false; // The player is not grounded by default

            // Here we are checking if the player is touching ground by creating a sphere around the Ground Check position with the
            // radius of Grounded Radius, and if the sphere overlaps with an object of the What Is Ground layer, then the player is grounded.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(characterController2D.groundCheck.position, characterController2D.groundedRadius, characterController2D.whatIsGround);
            foreach (var t in colliders){
                if (t.gameObject != gameObject) // excluding the component bearer
                    grounded = true;
            }
            characterController2D.Move(_horizontalMove, jump, ref grounded);
            jump = false;
            if (transform.position.y < -25) { // Killing the player if they fall out of the map
                animator.SetBool(IsDead, true);
                Warning.ShowWarning("You are out of bounds!");
            }
        }

        private void OnTriggerEnter2D(Collider2D coll){ // WM_03
            if (coll.gameObject.layer == LayerMask.NameToLayer("Deadly")){
                animator.SetBool(IsDead, true); // The animation then calls HandlePlayerDeath() via an event
            }
        }

        // Animation Events can't call methods of other objects, so this is an intermediary method
        private void HandlePlayerDeath(){ // WM_F04
            levelManager.LostLife();
            animator.Play("Idle");
            animator.SetBool(IsDead, false);
            isRespawning = true;
        }

        public void Respawn(){ // WM_F05
            if (FindObjectOfType<LevelManager>().ide3D) {
                if (!GetComponent<TransitionablePair>()) {
                    Warning.ShowWarning("No paired object found!");
                    return;
                }
                GameObject player3D = GetComponent<TransitionablePair>().target.gameObject;
                player3D.transform.position = spawn.transform.position;
                player3D.transform.rotation = new Quaternion(0, 0, 0, 0);
                if (!_dimensionSwitcher){ 
                    _dimensionSwitcher = FindObjectOfType<DimensionSwitcher>();
                }
                _dimensionSwitcher.Clean2DWorld();
                _dimensionSwitcher.Slice3DWorld(player.transform.forward);
            }
        }
    }
}