using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
    public DimensionSwitcher dimensionSwitcher;
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    public GameObject ide2D;
    public GameObject ide3D;
    public GameObject Life3, Life2, Life1;
    public Text timeText;
    public Sprite lostLifeSprite;
    public bool isIn2D = false; //for now
    public int switchesPressed = 0;
    public int lives = 3;
    int seconds, minutes, startSeconds;
    string strMinutes = "0"; 
    string strSeconds = "0";
    public float TimePassed = 0;
    
    // Start is called before the first frame update
    void Start(){
        // lostLifeSprite = Resources.Load<Sprite>("Robotic/GUI_sci-fi.png");
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
        
        #region TimeCalculator
        TimePassed += Time.deltaTime;
        seconds = (int)TimePassed - (60 * minutes);
        if(seconds==60){
            minutes++;
            if(minutes>9) 
                strMinutes = minutes.ToString();
            else if (minutes < 9) 
                strMinutes = "0" + minutes;
        }
        if(seconds<=9) 
            strSeconds = "0" + seconds;
        else 
            strSeconds = seconds.ToString();
        timeText.text = strMinutes + ":" + strSeconds;
        #endregion
    }

    #region Dimension Switching
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
    #endregion
    
    public void SwitchPressed(){
        switchesPressed++;
        Debug.Log(switchesPressed + " Pressed");
    }
    public void LoadNextLevel() {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex)+1);
    }
}
