using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public bool isNewGame;

    private SaveData saveData;

    private const string saveFile = "SaveData.json";

    private void Awake()
    {
        Instance = this;

        if (DoesFileAlreadyExist())
            LoadSave();
        else
            CreateSave();
    }

    private string ComputePath()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFile);

        return path;
    }

    private bool DoesFileAlreadyExist()
    {
        return File.Exists(ComputePath());
    }

    private void DeleteSave()
    {
        if (DoesFileAlreadyExist())
            File.Delete(ComputePath());
    }

    private void CreateSave()
    {
        saveData = new SaveData();
        StoreData(saveData);
        isNewGame = true;
    }

    private void LoadSave()
    {
        saveData = LoadData<SaveData>();
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }
    
    private bool StoreData<T>(T data)
    {
        string path = ComputePath();

        try
        {
            if (DoesFileAlreadyExist())
                DeleteSave();

            using FileStream stream = File.Create(path);
            stream.Close();

            string text = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            File.WriteAllText(path, text);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    private T LoadData<T>()
    {
        string path = ComputePath();

        if (!DoesFileAlreadyExist())
        {
            Debug.Log("SaveFile not found");
            throw new FileNotFoundException(path);
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }
    }

    private void OnApplicationQuit()
    {
        StoreData(saveData);
    }
}
