using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject battlefield;
    private LevelData currentLevelData;
    private Vector3Int enemySpawnCell;

    private GridCell[,] gridCells;
    private int tilemapWidth;
    private int tilemapHeight;
    private int tilemapStartX;
    private int tilemapStartY;

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
        string[] tiledata = currentLevelData.tiledata;
        Dictionary<string, string> tilemap = currentLevelData.tilemap;

        this.tilemap.ClearAllTiles();

        tilemapWidth = tiledata[0].Length;
        tilemapHeight = tiledata.Length;
        tilemapStartX = -tilemapWidth / 2;
        tilemapStartY = tilemapHeight / 2 - 1;

        gridCells = new GridCell[tilemapWidth, tilemapHeight];
        for (int rowIndex = 0; rowIndex < tiledata.Length; rowIndex++)
        {
            string line = tiledata[rowIndex];
            for (int charCol = 0; charCol < line.Length; charCol++)
            {
                char tileChar = line[charCol];
                string tilePath = tilemap[tileChar.ToString()];
                TileBase tileBase = Resources.Load<TileBase>(tilePath);
                if (tileBase)
                {
                    Vector3Int cellPosition = new Vector3Int(tilemapStartX + charCol, tilemapStartY - rowIndex, 0);
                    GridCell gridCell = new GridCell(tileChar, tileBase, cellPosition);
                    gridCells[charCol, rowIndex] = gridCell;

                    this.tilemap.SetTile(cellPosition, tileBase);
                }
            }
        }
    }

    private void CalculateEnemyPath()
    {
        enemySpawnCell = new Vector3Int(0, 0, 0);
    }

    public bool IsTileAvailable(Vector3 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        Vector2Int gridPosition = new Vector2Int(cellPosition.x - tilemapStartX, tilemapStartY - cellPosition.y);

        GridCell gridCell = gridCells[gridPosition.x, gridPosition.y];
        if (gridCell != null)
        {
            return gridCell.isAvailable();
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

        Vector2Int gridPosition = new Vector2Int(cellPosition.x - tilemapStartX, tilemapStartY - cellPosition.y);
        gridCells[gridPosition.x, gridPosition.y].tower = tower;

        tower.Activate();
    }

    public void ColorCell(Vector3Int cellPosition, Color color)
    {
        tilemap.SetTileFlags(cellPosition, TileFlags.None);
        tilemap.SetColor(cellPosition, color);
    }

    internal Vector3Int GetTileCellPosition(Vector3 mousePosition)
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
