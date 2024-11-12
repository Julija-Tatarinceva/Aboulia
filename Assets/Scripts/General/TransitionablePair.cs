using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionablePair : MonoBehaviour {
    // This code currently is only used for the player objects, since they cannot be created from one object both in 2D and 3D
    // The 3D and 2D models have to be teleported to new locations after transitioning
    public TransitionablePair target; // A clone of this object in the other dimension
    private Vector3 posBeforeTransition;
    private float moveDistance;
    public Vector3 direction;
    private void Start(){
        posBeforeTransition = transform.position;
    }

    public void Update() {
        Debug.DrawRay(transform.position, direction * 20, Color.red);
        // Debug.Log(transform.position + direction.ToString());
    }

    public void BeginTransition(Vector3 planeRight){ // Getting position of the first clone in the 2D
        Vector3 changeInPosition = transform.position - posBeforeTransition;
        
        // Calculate the direction vector from posBeforeTransition to planeRight (x and z only)
        target.direction = -planeRight.normalized;
            
        changeInPosition.z = 0; // Does not matter a lot, but if moved too far away a 2D object can be out of range for the camera
        // Apply this change in position to the target
        target.FinishTransition(changeInPosition);
        posBeforeTransition = transform.position;
    } 
    public void BeginTransition(){ // Getting position of the first clone in the 3D
        // Calculate the distance the player has moved along the x-axis
        moveDistance = transform.position.x - posBeforeTransition.x;
        
        // Calculate the change in position, keeping y intact
        // We need to move the 3D character along the x-axis on the slicing plane, since in the 2D the player moves along the default axes.
        Vector3 changeInPosition = new Vector3(
            direction.x * moveDistance,
            transform.position.y - posBeforeTransition.y,  // Keep the y component difference
            direction.z * moveDistance
        );
        
        target.FinishTransition(changeInPosition);       
        posBeforeTransition = transform.position; // Next transition the current position will be used as the first point
    } 
    private void FinishTransition(Vector3 changeInPosition){ // Moving the clone in the dimension player is switching IN to
        transform.position += changeInPosition; // Move the player along this new position
        posBeforeTransition = transform.position; // Update posBeforeTransition for the next transition
    }
}