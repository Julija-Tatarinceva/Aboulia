using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionablePair : MonoBehaviour
{
    public TransitionablePair target; // A clone of this object in the other dimension
    
    public void StartTransition(){ // Getting position of the first clone in the dimension player is switching OUT of
        target.ReceiveTransition(transform);        
    }
    public void ReceiveTransition(Transform newTransform){ // Moving the clone in the dimension player is switching IN to
        Vector3 pos = new Vector3(newTransform.position.x, newTransform.position.y, 0f);
        transform.position = pos;
    }
}
