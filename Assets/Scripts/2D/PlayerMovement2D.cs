using UnityEngine;

public class PlayerMovement2D : MonoBehaviour {
    // For more effectiveness using hashes instead of strings
    private static readonly int VerticalMove = Animator.StringToHash("VerticalMove");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    
    public CharacterController2D characterController2D;
    public LevelManager levelManager;
    private float _horizontalMove = 0f;
    public bool jump;
    public Animator animator;
    public Rigidbody2D playerRigidbody2D;
    public GameObject player, spawn;
    public SoundPlayer soundPlayer;
    
    private void Update(){
        // Using keys as input, these keys can be changed in project's settings
        _horizontalMove = Input.GetAxisRaw("Horizontal");
        animator.SetFloat(Speed, Mathf.Abs(_horizontalMove));
        animator.SetFloat(VerticalMove, playerRigidbody2D.velocity.y);
        animator.SetBool(IsGrounded, characterController2D.grounded);
        if (Input.GetButtonDown("Jump") && !jump){
            jump = true;
        }
        soundPlayer.PlayStepSound(Mathf.Abs(_horizontalMove) > 0, characterController2D.grounded);
    }
    // Since movement is a physics operation, it should be done through Fixed Update, which is not tied to frame rate
    // This helps in avoiding collision and speed calculation mistakes
    private void FixedUpdate(){
        characterController2D.Move(_horizontalMove, jump);
        jump = false;
    }

    private void OnTriggerEnter2D(Collider2D coll){
        if (coll.gameObject.layer == LayerMask.NameToLayer("Deadly")) {
            animator.SetBool(IsDead, true); // The animation then calls HandlePlayerDeath() via an event
        }
    }

    // Animation Events can't call methods of other objects, so this is an intermediary method
    private void HandlePlayerDeath(){ 
        levelManager.LostLife();
        animator.Play("Idle");
        animator.SetBool(IsDead, false);
    }

    public void Respawn() {
        // if (FindObjectOfType<LevelManager>().levelIncludes3D == true)
        // {
        //     if (Player3D.activeSelf)
        //     {
        //         FindObjectOfType<MController3D>().isGrounded = true;
        //         FindObjectOfType<MController3D>().dead = false;
        //         Player3D.transform.position = Spawn3D.transform.position;
        //     }
        // }
        player.transform.position = spawn.transform.position;
    }
}