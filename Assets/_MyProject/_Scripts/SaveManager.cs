//using System;
//using System.IO;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class GameData
//{
//    public List<string> completedTimelines = new List<string>();
//    // Later: add NPC dialog states, quests, inventory, etc.
//}

//public class SaveManager : MonoBehaviour
//{
//    public static SaveManager Instance;

//    private string saveFilePath;
//    private GameData currentData;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
//            LoadGame();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    // --- Timeline Handling ---
//    public bool HasTimelinePlayed(string key)
//    {
//        return currentData.completedTimelines.Contains(key);
//        Debug.Log("HasTimelinePlayed is being called");
//    }

//    public void MarkTimelineAsPlayed(string key)
//    {
//        if (!currentData.completedTimelines.Contains(key))
//        {
//            currentData.completedTimelines.Add(key);
//            SaveGame();
//        }
//    }

//    // --- Save / Load ---
//    public void SaveGame()
//    {
//        try
//        {
//            string json = JsonUtility.ToJson(currentData, true);
//            File.WriteAllText(saveFilePath, json);
//            Debug.Log($"💾 Game saved to {saveFilePath}");
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("❌ Save failed: " + e.Message);
//        }
//    }

//    public void LoadGame()
//    {
//        if (File.Exists(saveFilePath))
//        {
//            try
//            {
//                string json = File.ReadAllText(saveFilePath);
//                currentData = JsonUtility.FromJson<GameData>(json);
//                if (currentData == null) currentData = new GameData();
//                Debug.Log("📂 Game data loaded.");
//            }
//            catch (Exception e)
//            {
//                Debug.LogError("❌ Load failed: " + e.Message);
//                currentData = new GameData();
//            }
//        }
//        else
//        {
//            Debug.Log("🆕 No save found. Creating new save file.");
//            currentData = new GameData();
//            SaveGame();
//        }
//    }

//    public void ResetSave()
//    {
//        currentData = new GameData();
//        SaveGame();
//        Debug.Log("🧹 Save data reset.");
//    }
//}
