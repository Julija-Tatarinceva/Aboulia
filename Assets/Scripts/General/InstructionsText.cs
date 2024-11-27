using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsText : MonoBehaviour{
    public GameObject instructionsTextBox;
    static GameObject _instructionsInstance;
    static GameObject _mainCanvas;
    
    public void SetActive(){
        if (!instructionsTextBox){
            if(!_instructionsInstance) _instructionsInstance = GameObject.Find("Instructions Instance");
            if(!_mainCanvas) _mainCanvas = GameObject.Find("ingame text");
            instructionsTextBox = Instantiate(_instructionsInstance, _mainCanvas.transform);
            instructionsTextBox.transform.position = transform.position;
            TextMeshProUGUI text = instructionsTextBox.GetComponent<TextMeshProUGUI>();
            if (LevelManager.language == 0) {
                text.text = "Press " + LevelManager.interactButton + " to interact";
            }
            else if(LevelManager.language == 1) {
                text.text = "Spiediet " + LevelManager.interactButton + " lai mijiedarbotos";
            }
            else if(LevelManager.language == 2) {
                text.text = "Нажмите " + LevelManager.interactButton + " для взаимодействовия";
            }
            float height = GetComponent<BoxCollider2D>().size.y; // Getting the height of the trigger collider
            instructionsTextBox.transform.position = new Vector2(transform.position.x, transform.position.y + height/2);
        }
        else 
            instructionsTextBox.SetActive(true);
    }

    public void SetInactive(){
        instructionsTextBox.SetActive(false);
    }
}
