using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionsText : MonoBehaviour{
    public GameObject instructionsTextBox;
    static GameObject _instructionsInstance;
    static GameObject _mainCanvas;
    private float height;
    
    public void SetActive(){
        if (!instructionsTextBox) // Create instructions for the first time
            InstantiateText();
        else {
            // The instructions for this object have already been generated, we just need to reposition them
            instructionsTextBox.SetActive(true);
            instructionsTextBox.transform.position = new Vector2(transform.position.x, transform.position.y + height);
        }
    }

    public void SetInactive(){
        if (!instructionsTextBox) // Create instructions for the first time
            InstantiateText();
        instructionsTextBox.SetActive(false);
    }

    private void InstantiateText() {
        // Canvas and the instruction template only need to be found once for this class
        if(!_instructionsInstance) _instructionsInstance = GameObject.Find("InstructionsParent");
        if(!_mainCanvas) _mainCanvas = GameObject.Find("ingame text");
            
        // Copy and reposition the template
        instructionsTextBox = Instantiate(_instructionsInstance, _mainCanvas.transform); // the copy has Canvas as the parent
        height = GetComponent<BoxCollider2D>().size.y; // Getting the height of the trigger collider
        instructionsTextBox.transform.position = new Vector2(transform.position.x, transform.position.y + height);
            
        // Getting the right text in, language is determined by the Level Manager
        TextMeshProUGUI text = instructionsTextBox.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (LevelManager.Language == 0) {
            text.text = "Press " + LevelManager.InteractButton + " to interact"; //English
        }
        else if(LevelManager.Language == 1) {
            text.text = "Spiediet " + LevelManager.InteractButton + " lai mijiedarbotos"; //Latvian
        }
        else if(LevelManager.Language == 2) {
            text.text = "Нажмите " + LevelManager.InteractButton + " для взаимодействовия"; //Russian
        }
    }
}
