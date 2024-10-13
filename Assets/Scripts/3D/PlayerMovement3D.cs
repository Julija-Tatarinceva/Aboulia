using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour {
    public CharacterController characterController;
    public float speed = 8.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    float targetAngle;
    float verticalSpeed;
    Vector3 moveDirection;
    public Vector3 velocity;
    public Transform cam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized; // Turning the inputs into a normalized vector
        if (moveDirection.magnitude >= 0.1f) { // We can proceed if the vector is not a zero vector
            targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // This angle is used to figure out which way the player wants to go relative to the camera rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // Calculating how much the current rotation has to be changed to reach the new one
            transform.rotation = Quaternion.Euler(0f, angle, 0f); // Turning the player's body in the direction it's moving
            
            // The vector of movement is now calculated by rotating a forward vector the same way the player has just been rotated
            Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; 
            // The controller component handles the movement of the body
            characterController.Move(direction.normalized * (speed * Time.deltaTime));
            // characterController.Move(velocity * Time.deltaTime); // not much of a difference
        }
    
    }
}
