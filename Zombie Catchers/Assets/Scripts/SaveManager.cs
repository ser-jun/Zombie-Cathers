using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string filePath;
    public static SaveManager Instance;
  
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject);}
        filePath = Application.persistentDataPath + "/GameData.txt";
    }

    public void SaveData(GameData data)
    {
     string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

    }
   public GameData LoadData()
    {
        if (File.Exists(filePath)) 
        {
        string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return new GameData();
       
    }
   
}
