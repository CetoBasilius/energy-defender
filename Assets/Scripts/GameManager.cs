using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public Tilemap backgroundTilemap;
    private static TowersData towersData;

    void Start()
    {
        LevelManager.Setup(backgroundTilemap);
        LevelManager.SetLevel("World01/01"); // TODO: This will be able to be set from the main menu
    }

    void Update()
    {
        
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadTowerData()
    {
        try
        {
            TextAsset levelDataString = Resources.Load<TextAsset>("Data/towers");
            towersData = JsonConvert.DeserializeObject<TowersData>(levelDataString.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error loading tower data:" + e);
        }
    }
    
    public TowerData GetTowerData(string dataType)
    {
        return towersData.ContainsKey(dataType) ? towersData[dataType] : null;
    }
}
