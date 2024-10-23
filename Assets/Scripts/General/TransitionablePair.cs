using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionablePair : MonoBehaviour {
    // This code currently is only used for the player objects, since they cannot be created from one object both in 2D and 3D
    // The 3D and 2D models have to be teleported to new locations after transitioning
    public TransitionablePair target; // A clone of this object in the other dimension
    private Vector3 posBeforeTransition;
    private void Start(){
        posBeforeTransition = transform.position;
    }
    public void BeginTransition(){ // Getting position of the first clone in the dimension player is switching OUT of
        Vector3 changeInPosition = transform.position - posBeforeTransition;
        changeInPosition.z = 0; // Does not matter a lot, but if moved too far away a 2D object can be out of range for cameras
        target.FinishTransition(changeInPosition);       
        posBeforeTransition = transform.position; // Next transition the current position will be used as the first point
    } 
    private void FinishTransition(Vector3 changeInPosition){ // Moving the clone in the dimension player is switching IN to
        transform.position = transform.position + changeInPosition;
        posBeforeTransition = transform.position; // Next transition the current position will be used as the first point
    }
}