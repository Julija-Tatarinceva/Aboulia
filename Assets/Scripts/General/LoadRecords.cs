using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class LoadRecords : MonoBehaviour
{
    public Text level1Record;
    public Text level2Record;
    public Text level3Record;
    public Text level4Record;
    public Text loadGame;
    void Start() {
        LocalizeStringEvent localizeEvent;
        if (PlayerPrefs.HasKey("BestTimeLVL 1")) {
            localizeEvent = level1Record.GetComponent<LocalizeStringEvent>();
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                StringVariable record = new StringVariable();
                lvl.Value = 1;
                record.Value = PlayerPrefs.GetString("BestTimeLVL 1");
                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
                localizeEvent.StringReference.Add("record", record); // Set the key placeholder with the dynamic value
            }
        }
        if (PlayerPrefs.HasKey("BestTimeLVL 2")) {
            localizeEvent = level2Record.GetComponent<LocalizeStringEvent>();
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                StringVariable record = new StringVariable();
                lvl.Value = 2;
                record.Value = PlayerPrefs.GetString("BestTimeLVL 2");
                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
                localizeEvent.StringReference.Add("record", record); // Set the key placeholder with the dynamic value
            }
        }
        // level3Record.text = "Level 3: " + PlayerPrefs.GetString("Best time at level 3");
        // level4Record.text = "Level 4: " + PlayerPrefs.GetString("Best time at level 4");
        
        // Get the LocalizeStringEvent and bind the placeholder value
        localizeEvent = loadGame.GetComponent<LocalizeStringEvent>();
        if (PlayerPrefs.HasKey("Level where we stopped")) {
            localizeEvent.StringReference.TableEntryReference = "are_you_sure_to_load_text";
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                lvl.Value = PlayerPrefs.GetInt("SavedLevel");
                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
            }
        }
        else
            localizeEvent.StringReference.TableEntryReference = "no_saved_level_text";
        localizeEvent.RefreshString(); // Refresh the localized text to apply the changes
    }
}
