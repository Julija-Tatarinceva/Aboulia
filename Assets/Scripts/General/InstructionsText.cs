using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace General
{
    public class InstructionsText : MonoBehaviour{
        private static GameObject _instructionsTextBox;
        private static GameObject _instructionsInstance;
        private static GameObject _mainCanvas;
        private static float _height;

        public void Start() {
            // Canvas and the instruction template only need to be found once for this class
            if(!_instructionsInstance){ 
                _instructionsInstance = GameObject.Find("InstructionsParent");
            }
            if(!_mainCanvas){ 
                _mainCanvas = GameObject.Find("ingame text");
            }
        }

        public void SetActive(){ // TM_F01
            if (!_instructionsTextBox){ // Create instructions for the first time
                InstantiateText();
            }
            else {
                // The instructions for this object have already been generated, just need to reposition them
                _instructionsTextBox.SetActive(true);
                _instructionsTextBox.transform.position = new Vector2(transform.position.x, transform.position.y + _height/2);
            }
        }

        public void SetInactive(){ // TM_F02
            if (!_instructionsTextBox){ // Create instructions for the first time
                InstantiateText();
            }
            _instructionsTextBox.SetActive(false);
        }

        private void InstantiateText() { // TM_F03
            if (!_mainCanvas || !_instructionsInstance) {
                Warning.ShowWarning("Not able to create instructionsText");
                return;
            }
            
            // Copy and reposition the template
            _instructionsTextBox = Instantiate(_instructionsInstance, _mainCanvas.transform); // the copy must have Canvas as the parent
            _height = GetComponent<BoxCollider2D>().size.y; // Getting the height of the trigger collider
            _instructionsTextBox.transform.position = new Vector2(transform.position.x, transform.position.y + _height/2);
            
            // Get the LocalizeStringEvent and bind the placeholder value
            LocalizeStringEvent localizeEvent = _instructionsTextBox.transform.GetChild(0).gameObject.GetComponent<LocalizeStringEvent>();
            // Add the "key" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                StringVariable key = new StringVariable {
                    Value = FindObjectOfType<LevelManager>().interactButton
                };
                localizeEvent.StringReference.Add("key", key); // Set the key placeholder with the dynamic value
                localizeEvent.RefreshString(); // Refresh the localized text to apply the changes
            }
        }
    }
}
