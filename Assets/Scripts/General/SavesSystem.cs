using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class PlayerData {
    public int LastLevel;
    public List<int> LevelRecords; // A list to store level records
}

public class SavesSystem : MonoBehaviour {
    private PlayerData _playerData;
    private string _saveFilePath;
    private void Start() {
        _playerData = new PlayerData();
        _saveFilePath = Application.persistentDataPath + "/PlayerData.json";
        Debug.Log(Application.persistentDataPath);
        
        // Initialize the player data structure
        _playerData.LastLevel = 0; 
        _playerData.LevelRecords = new List<int>();
        
        // Load data if it exists
        LoadData();
        // Populate level records automatically based on the number of scenes
        InitializeLevelRecords();
    }

    private void InitializeLevelRecords() {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings-1;

        // Ensure the list is the correct size for the number of levels
        while (_playerData.LevelRecords.Count < sceneCount)
            _playerData.LevelRecords.Add(0);
    }

    private void SaveData() {
        string json = JsonUtility.ToJson(_playerData);
        File.WriteAllText(_saveFilePath, json);
        Debug.Log("Save file created at: " + _saveFilePath);
    }
    private void LoadData() {
        if (File.Exists(_saveFilePath)) {
            string json = File.ReadAllText(_saveFilePath);
            _playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Load game complete! \nLast level: " + _playerData.LastLevel + " Last level: " + _playerData.LevelRecords[0]);
        }
        else
            Debug.Log("There is no save files to load!");
    }
    public void UpdateLevelRecord(int levelIndex, int record) {
        // Ensure the record list is large enough
        InitializeLevelRecords();

        if (levelIndex > 0 && levelIndex <= _playerData.LevelRecords.Count) {
            _playerData.LevelRecords[levelIndex - 1] = record;
            SaveData();
        }
    }

    public void UpdateLastSavedLevel(int levelIndex) {
        if (levelIndex > 0 && levelIndex <= _playerData.LevelRecords.Count) {
            _playerData.LastLevel = levelIndex;
            SaveData();
        }
    }
    public int GetLevelRecord(int levelIndex) {
        if (levelIndex > 0 && levelIndex < _playerData.LevelRecords.Count)
            return _playerData.LevelRecords[levelIndex-1];
        return 0;
    }

    public int GetLastSavedLevel() {
        return _playerData.LastLevel;
    }

    public int LoadLastLevel() {
        return _playerData.LastLevel;
    }
    public void DeleteSaveFile() {
        if (File.Exists(_saveFilePath)) {
            File.Delete(_saveFilePath);
            Debug.Log("Save file deleted!");
        }
        else
            Debug.Log("There is nothing to delete!");
    }
}
