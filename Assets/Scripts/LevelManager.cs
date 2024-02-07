using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    private bool initialized = false;
    private LevelData currentLevelData;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    internal static void CreateInstance()
    {
        if (!instance)
        {
            GameObject levelManagerObject = new GameObject("LevelManager", typeof(LevelManager));
            DontDestroyOnLoad(levelManagerObject);
            levelManagerObject.transform.SetParent(null);
        }
    }

    private void Awake()
    {
        this.Initialize();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void Initialize()
    {
        if (!this.initialized)
        {
            if (!instance)
            {
                instance = this;
            }
        }
    }

    private bool LoadLevelData(string levelName = "01")
    {
        try
        {
            TextAsset levelDataString = Resources.Load<TextAsset>("Data/Levels/" + levelName);
            this.currentLevelData = JsonConvert.DeserializeObject<LevelData>(levelDataString.text);
            this.currentLevelData.name = levelName;

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error loading level data for " + levelName + ": " + e.Message);
        }
        return false;
    }

    public static bool SetLevel(string levelName)
    {
        if (instance == null)
        {
            Debug.LogError("LevelManager not initialized");
            return false;
        }


        if (!instance.LoadLevelData(levelName))
        {
            return false;
        }

        return true;
    }

    public static LevelData GetLevelData()
    {
        if (instance == null)
        {
            Debug.LogError("LevelManager not initialized");
            return null;
        }

        if (instance.currentLevelData == null)
        {
            Debug.LogError("No level data loaded");
            return null;
        }

        return instance.currentLevelData;
    }
}
