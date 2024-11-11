using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    public GameObject ide2D;
    public GameObject ide3D;
    public int switchAmount = 0;
    public bool isIn2D = false; //for now
    // Start is called before the first frame update
    void Start(){
        // Invoke("SwitchTo3D", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.T) && isIn2D)
        // {
        //     Debug.Log("yay");
        //     SwitchTo3D();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     Debug.Log("no");
        // }
    }

    public void SwitchTo2D(){
        ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D)
        {
            pair.BeginTransition();
        }
        ide3D.SetActive(false);
        isIn2D = true;
    }

    public void SwitchTo3D(Vector3 planeRight){
        ide3D.SetActive(true);
        foreach (var pair in transitionablePairs2D)
        {
            pair.BeginTransition();
        }
        ide2D.SetActive(false);
        isIn2D = false;
    }

    public void SwitchPressed(){
        switchAmount++;
        Debug.Log(switchAmount + " Pressed");
        if (switchAmount == 2) {
            Debug.Log("Door opened");
        }
    }
}
