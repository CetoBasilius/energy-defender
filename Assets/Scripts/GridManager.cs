using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject battlefield;
    private LevelData currentLevelData;
    private Vector3Int enemySpawnCell;
    private LinkedList<Vector3Int> enemyPath;


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
        this.CalculateEnemyPath();
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

        int lastCol = tilemapStartX + tilemapWidth;

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
                    if (tileChar == 'P' && charCol == lastCol)
                    {
                        enemySpawnCell = new Vector3Int(charCol -1, rowIndex - 1, 0);
                        Debug.Log("Enemy spawn cell: " + enemySpawnCell);
                    }
                }
            }
        }
    }

    private void CalculateEnemyPath()
    {
        // Check neighbor cells and add to path if available, ingore previous cell
    }

    public bool IsTileAvailable(Vector3 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        TileBase tilebase = tilemap.GetTile(cellPosition);

        if (tilebase == null)
        {
            return false;
        }

        // TODO: Implement available tiles (grass and not already occupied by a tower)
        if (tilebase.name == "grass")
        {
            return true;
        }

        return false;
    }

    public void AddTower(GameObject tower)
    {
        tower.transform.SetParent(battlefield.transform, false);
    }

    public void PlaceTower(Vector3 mousePosition, Tower tower)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPosition);
        snapPosition.z = tower.transform.position.z;
        tower.transform.position = snapPosition;

        // TODO: Set tile to tower so no other tower can be placed there or improve placement logic
        // TODO: Activate tower
    }

    public void ColorCell(Vector3Int cellPosition, Color color)
    {
        tilemap.SetTileFlags(cellPosition, TileFlags.None);
        tilemap.SetColor(cellPosition, color);
    }

    internal Vector3Int GetTileCell(Vector3 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        return cellPosition;
    }

    internal void AddEnemy(GameObject enemy)
    {
        enemy.transform.SetParent(battlefield.transform, false);
        enemy.transform.position = tilemap.GetCellCenterWorld(enemySpawnCell);
        enemy.transform.position.Set(enemy.transform.position.x, enemy.transform.position.y, -1);
    }
}
