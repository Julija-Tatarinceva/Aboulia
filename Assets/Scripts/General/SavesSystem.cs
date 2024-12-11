using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class PlayerData {
    public int lastLevel;
    public List<int> levelRecords; // A list to store level records
}

public class SavesSystem : MonoBehaviour {
    public PlayerData playerData;
    string saveFilePath;
    private void Start() {
        playerData = new PlayerData();
        saveFilePath = Application.persistentDataPath + "/PlayerData.json";
        Debug.Log(Application.persistentDataPath);
        
        // Initialize the player data structure
        playerData.lastLevel = 0; 
        playerData.levelRecords = new List<int>();
        
        // Load data if it exists
        LoadData();
        // Populate level records automatically based on the number of scenes
        InitializeLevelRecords();
    }
    void InitializeLevelRecords() {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings-1;

        // Ensure the list is the correct size for the number of levels
        while (playerData.levelRecords.Count < sceneCount) {
            playerData.levelRecords.Add(0);
        }
    }
    public void SaveData() {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Save file created at: " + saveFilePath);
    }
    private void LoadData() {
        if (File.Exists(saveFilePath)) {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Load game complete! \nLast level: " + playerData.lastLevel + " Last level: " + playerData.levelRecords[0]);
        }
        else
            Debug.Log("There is no save files to load!");
    }
    public void UpdateLevelRecord(int levelIndex, int record) {
        // Ensure the record list is large enough
        InitializeLevelRecords();

        if (levelIndex > 0 && levelIndex <= playerData.levelRecords.Count) {
            playerData.levelRecords[levelIndex - 1] = record;
            SaveData();
        }
    }

    public void UpdateLastSavedLevel(int levelIndex) {
        if (levelIndex > 0 && levelIndex <= playerData.levelRecords.Count) {
            playerData.lastLevel = levelIndex;
            SaveData();
        }
    }
    public int GetLevelRecord(int levelIndex) {
        if (levelIndex > 0 && levelIndex < playerData.levelRecords.Count)
            return playerData.levelRecords[levelIndex-1];
        return 0;
    }

    public int GetLastSavedLevel() {
        return playerData.lastLevel;
    }

    public int LoadLastLevel() {
        return playerData.lastLevel;
    }
    public void DeleteSaveFile() {
        if (File.Exists(saveFilePath)) {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted!");
        }
        else
            Debug.Log("There is nothing to delete!");
    }
}
