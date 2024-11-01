using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour {
    private bool _inRange = false;
    // Start is called before the first frame update
    void OnCollisionEnter2D() {
        _inRange = true;
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.E) && _inRange) {
            GetComponent<LevelManager>().SwitchPressed();
        }
    }
}
