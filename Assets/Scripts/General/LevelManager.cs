using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    public GameObject ide2D;
    public GameObject ide3D;
    // Start is called before the first frame update
    void Start(){
        Invoke("SwitchTo3D", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchTo2D(){
        Invoke("SwitchTo3D", 5f);
        ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D) {
            pair.StartTransition();
        }
        ide3D.SetActive(false);
    }

    public void SwitchTo3D(){
        Invoke("SwitchTo2D", 5f);
        ide2D.SetActive(false);
        foreach (var pair in transitionablePairs2D) {
            pair.StartTransition();
        }
        ide3D.SetActive(true);
    }
}
