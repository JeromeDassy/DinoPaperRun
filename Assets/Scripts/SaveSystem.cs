using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(GameData gameData)
    {
        string path = GetSaveFilePath();
        Debug.Log($"GameData path: {path}");

        // Create the directory if it doesn't exist
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameData);
        }

        // Check if the file exists after saving
        if (!File.Exists(path))
        {
            Debug.Log("Save failed");
            //GameManager.Instance.ShowErrorOnSave();
        }
    }


    public static GameData LoadGame()
    {
        string path = GetSaveFilePath();
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                GameData data = formatter.Deserialize(stream) as GameData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            //GameManager.Instance.ShowErrorOnSave();
            return null;
        }
    }

    public static void DeleteGame()
    {
        string path = GetSaveFilePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.LogWarning("Save file not found, deletion aborted.");
        }
    }

    private static string GetSaveFilePath()
    {
#if UNITY_PSP2 && !UNITY_EDITOR
        return "ux0:data/OrbitalFlagStudio/DinoPaperRun/PlayerData.ofs";
#endif
#if !UNITY_PSP2 || UNITY_EDITOR || UNITY_STANDALONE
        return Application.persistentDataPath + "/PlayerData.ofs";
#endif
    }
}
