using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public CharacterController2D characterController2D;
    public float runSpeed = 50f;
    private float _horizontalMove = 0f;
    public bool jump = false;

    public GameObject player;
    private void Update()
    {
        // Using keys as input, these keys can be changed in project's settings
        _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump") && !jump){
            jump = true;
        }
    }
    // Since movement is a physics operation, it should be done through Fixed Update, which is not tied to frame rate
    // This helps in avoiding collision and speed calculation mistakes
    private void FixedUpdate(){
        characterController2D.Move(_horizontalMove  * Time.fixedDeltaTime, jump);
        jump = false;
    }
    
}