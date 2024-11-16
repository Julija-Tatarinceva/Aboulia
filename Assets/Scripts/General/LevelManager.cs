using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public DimensionSwitcher dimensionSwitcher;
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    public GameObject ide2D;
    public GameObject ide3D;
    public int switchesPressed = 0;
    public bool isIn2D = false; //for now
    // Start is called before the first frame update
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.T) && isIn2D == false) { // Trigger dimension switch to 2D world
            Vector3 planeRight = dimensionSwitcher.Slice3DWorld();
            SwitchTo2D(planeRight);
        }
        else if (Input.GetKeyDown(KeyCode.T) && isIn2D) { // Trigger dimension switch to 3D world
            SwitchTo3D();
            dimensionSwitcher.Clean2DWorld();
        }
    }

    public void SwitchTo2D(Vector3 planeRight){
        ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D)
        {
            pair.BeginTransition(planeRight);
        }
        ide3D.SetActive(false);
        isIn2D = true;
    }

    public void SwitchTo3D(){
        ide3D.SetActive(true);
        foreach (var pair in transitionablePairs2D)
        {
            pair.BeginTransition();
        }
        ide2D.SetActive(false);
        isIn2D = false;
    }

    public void SwitchPressed(){
        switchesPressed++;
        Debug.Log(switchesPressed + " Pressed");
        if (switchesPressed == 2) {
            Debug.Log("Door opened");
        }
    }
}
