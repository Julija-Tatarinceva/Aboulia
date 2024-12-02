using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

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
            
        // Get the LocalizeStringEvent and bind the placeholder value
        LocalizeStringEvent localizeEvent = instructionsTextBox.transform.GetChild(0).gameObject.GetComponent<LocalizeStringEvent>();
        // Add the "key" parameter to the LocalizeStringEvent
        if (localizeEvent != null) {
            StringVariable key = null;
            key = new StringVariable();
            key.Value = LevelManager.InteractButton;
            // Set the key placeholder with the dynamic value
            localizeEvent.StringReference.Add("key", key);

            // Refresh the localized text to apply the changes
            localizeEvent.RefreshString();
        }
    }
}
