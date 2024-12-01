using UnityEngine;

public class TransitionablePair : MonoBehaviour {
    // This code currently is used for the player and interactable objects, since they cannot be created from one object both in 2D and 3D
    // The 3D and 2D models have to be teleported to new locations after transitioning
    public TransitionablePair target; // A clone of this object in the other dimension
    private Vector3 _posBeforeTransition;
    private float _moveDistance;
    public Vector3 direction;
    private void Start(){
        _posBeforeTransition = transform.position;
    }
    
    public void BeginTransition(Vector3 planeRight){ // Beginning transition from the 3D
        if (CompareTag("Interactable")) {
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y+0.35f, target.transform.position.z);
            return;
        }
            
        Vector3 changeInPosition = new Vector3(
            transform.position.x,
            target.transform.position.y,  // Keep the y component difference
            0
        );
        
        target.FinishTransition(changeInPosition, planeRight);
    } 
    public void BeginTransition(){ // Beginning transition from the 2D
        // Calculate the distance the player has moved along the x-axis
        _moveDistance = transform.position.x - _posBeforeTransition.x;
        
        // Calculate the change in position, keeping y intact
        // We need to move the 3D character along the x-axis on the slicing plane, since in the 2D the player moves along the default axes.
        Vector3 changeInPosition = new Vector3(
            direction.x * _moveDistance,
            transform.position.y - _posBeforeTransition.y,  // Keep the y component difference
            direction.z * _moveDistance
        );
        if (CompareTag("Interactable")) 
            gameObject.SetActive(false);
        else
            target.FinishTransition(changeInPosition);      
    } 
    private void FinishTransition(Vector3 changeInPosition){ // Moving the 2D clone when the player is switching to 2D
        transform.position += changeInPosition; // Move the player along this new position
        _posBeforeTransition = transform.position; // Update posBeforeTransition for the next transition
    }

    private void FinishTransition(Vector3 changeInPosition, Vector3 planeRight) { // Moving the 3D clone when the player is switching to 3D
        // Calculate the direction vector from posBeforeTransition to planeRight (x and z only)
        direction = -planeRight.normalized;
        
        // Apply this change in position to the target
        transform.position = changeInPosition;
        _posBeforeTransition = transform.position; // Update posBeforeTransition for the next transition
    }
}