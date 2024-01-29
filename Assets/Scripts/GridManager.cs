using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    private LevelData currentLevelData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(LevelData levelData)
    {
        this.currentLevelData = levelData;
        this.BuildTilemap();
    }

    private void BuildTilemap()
    {
        string[] tiledata = this.currentLevelData.tiledata;
        Dictionary<string, string> tilemap = this.currentLevelData.tilemap;

        this.tilemap.ClearAllTiles();

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
                    this.tilemap.SetTile(new Vector3Int(tilemapStartX + charCol, tilemapStartY - rowIndex, 0), tile);
                }
            }
        }
    }
}
