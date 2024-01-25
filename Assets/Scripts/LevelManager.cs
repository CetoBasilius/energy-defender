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
    private Tilemap backgroundTilemap;
    private Tilemap towerTilemap;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    internal static void CreateInstance()
    {
        if (!instance)
        {
            GameObject gameManagerObject = new GameObject("LevelManager", typeof(LevelManager));
            DontDestroyOnLoad(gameManagerObject);
            gameManagerObject.transform.SetParent(null);
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

    private void buildBackgroundTilemap()
    {
        string[] tiledata = this.currentLevelData.tiledata;
        Dictionary<string, string> tilemap = this.currentLevelData.tilemap;

        this.backgroundTilemap.ClearAllTiles();
        this.towerTilemap.ClearAllTiles();

        int tilemapWidth = tiledata[0].Length;
        int tilemapHeight = tiledata.Length;

        int tilemapStartX = -tilemapWidth / 2;
        int tilemapStartY = tilemapHeight / 2 - 1;

        for (int rowIndex = 0; rowIndex < tiledata.Length; rowIndex++)
        {
            string line = tiledata[rowIndex];
            for (int charCol = 0; charCol < line.Length; charCol++)
            {
                char tileChar = line[charCol];
                TileBase tile = Resources.Load<TileBase>(tilemap[tileChar.ToString()]);
                if (tile)
                {
                    this.backgroundTilemap.SetTile(new Vector3Int(tilemapStartX + charCol, tilemapStartY - rowIndex, 0), tile);
                }
            }
        }
    }

    private bool LoadLevelData(string levelName = "01")
    {
        try
        {
            TextAsset levelDataString = Resources.Load<TextAsset>("Data/Levels/" + levelName);
            this.currentLevelData = JsonConvert.DeserializeObject<LevelData>(levelDataString.text);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error loading level data:" + e);
        }
        return false;
    }

    private void SetTilemaps(Tilemap backgroundTilemap, Tilemap towerTilemap)
    {
        this.backgroundTilemap = backgroundTilemap;
        this.towerTilemap = towerTilemap;
    }

    internal static bool SetLevel(string levelName)
    {
        if (instance == null)
        {
            Debug.LogError("LevelManager not initialized");
            return false;
        }

        if (!instance.towerTilemap || !instance.backgroundTilemap)
        {
            Debug.Log("Tilemaps are not set in the inspector");
            return false;
        }

        if (!instance.LoadLevelData(levelName))
        {
            return false;
        }

        instance.buildBackgroundTilemap();

        return true;
    }

    internal static void Setup(Tilemap backgroundTilemap, Tilemap towerTilemap)
    {
        if (instance)
        {
            instance.SetTilemaps(backgroundTilemap, towerTilemap);
        }
    }
}
