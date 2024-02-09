using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : UnitFactory
{
    public Tilemap tilemap;
    public GameObject battlefield;
    public bool debugVisuals = false;

    private LevelData currentLevelData;
    private GridCell[,] gridCells;

    private int tilemapWidth;
    private int tilemapHeight;
    private int tilemapStartX;
    private int tilemapStartY;

    private GridPath gridPath;

    void Update()
    {

    }

    public void Setup(LevelData levelData)
    {
        currentLevelData = levelData;
        BuildTilemap();
        if (!gridPath.IsValid())
        {
            // TODO: Show error message?
        }
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

        gridPath = new GridPath();
        gridCells = new GridCell[tilemapWidth, tilemapHeight];
        for (int rowIndex = 0; rowIndex < tiledata.Length; rowIndex++)
        {
            string line = tiledata[rowIndex];

            gridPath.NewRow();
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

                    gridPath.ConnectHorizontalPathCells(gridCell, charCol == 0);
                }
            }
            gridPath.EndRow();
        }

        gridPath.ConnectVerticalPathCells();
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

    public void ColorCell(Vector3Int cellPosition, Color color, bool isDebug = false)
    {
        if (isDebug && !debugVisuals)
        {
            return;
        }
        tilemap.SetTileFlags(cellPosition, TileFlags.None);
        tilemap.SetColor(cellPosition, color);
    }

    internal Vector3Int GetTileCellPosition(Vector3 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        return cellPosition;
    }

    // Tower is added to the battlefield, but not placed yet
    public GameObject AddTower(string id)
    {
        GameObject tower = CreateTower(id);
        tower.transform.SetParent(battlefield.transform, false);

        return tower;
    }

    // Places an already added tower to the battlefield and activates it
    public void PlaceTower(Vector3 mousePosition, Tower tower)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        Vector3 snapPosition = tilemap.GetCellCenterWorld(cellPosition);
        snapPosition.z = tower.transform.position.z;
        tower.transform.position = snapPosition;

        Vector2Int gridPosition = new Vector2Int(cellPosition.x - tilemapStartX, tilemapStartY - cellPosition.y);
        GridCell gridCell = gridCells[gridPosition.x, gridPosition.y];
        gridCell.tower = tower;

        tower.Activate();
        tower.SetCurrentCell(gridCell);

        List<GridCell> cellsInRange = GetNeighbouringCells(cellPosition.x, cellPosition.y, tower.GetRange());
        foreach (GridCell cell in cellsInRange)
        {
            cell.towersWatching.Add(tower);
        }
    }

    private List<GridCell> GetNeighbouringCells(int cellX, int cellY, int range)
    {
        cellX -= tilemapStartX;
        cellY += tilemapStartY;
        cellY = tilemapHeight - cellY - 2;

        int startX = Mathf.Max(0, cellX - range);
        int endX = Mathf.Min(tilemapWidth - 1, cellX + range);
        int startY = Mathf.Max(0, cellY - range);
        int endY = Mathf.Min(tilemapHeight - 1, cellY + range);

        List<GridCell> cellsInRange = new List<GridCell>();
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (Vector2Int.Distance(new Vector2Int(cellX, cellY), new Vector2Int(i, j)) <= range)
                {
                    cellsInRange.Add(gridCells[i, j]);
                }
            }
        }

        return cellsInRange;
    }

    // Enemy is spawned at the spawn point on the battlefield
    public void SpawnEnemy(string id)
    {
        GameObject enemy = CreateEnemy(id);
        enemy.transform.SetParent(battlefield.transform, false);
        enemy.transform.position = tilemap.GetCellCenterWorld(gridPath.enemySpawn.cellPosition);
        enemy.transform.position.Set(enemy.transform.position.x, enemy.transform.position.y, -1);

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.OnCellChanged += HandleEnemyChangedCell;
        enemyComponent.SetCurrentCell(gridPath.enemySpawn);
    }

    private List<GridCell> HandleEnemyChangedCell(Enemy enemy, GridCell newCell, GridCell oldCell)
    {
        bool isNewTowerEnemy = false;
        bool isOldTowerEnemy = false;
        if (newCell.NotifyEnemyEntered(enemy))
        {
            isNewTowerEnemy = true;
        }
        if (oldCell != null)
        {
            if (oldCell.NotifyEnemyLeft(enemy))
            {
                isOldTowerEnemy = true;
            }
        }

        // Check surrounding cells for towers and return them in a list
        List<GridCell> cellsWithTowers = new List<GridCell>();
        List<GridCell> cellsInRange = GetNeighbouringCells(newCell.cellPosition.x, newCell.cellPosition.y, enemy.GetRange());
        foreach (GridCell cell in cellsInRange)
        {
            if (cell == newCell && isNewTowerEnemy)
            {
                ColorCell(newCell.cellPosition, Color.blue, true);
            }
            else if (cell == oldCell && isOldTowerEnemy)
            {
                ColorCell(oldCell.cellPosition, Color.yellow, true);
            }
            else
            {
                ColorCell(cell.cellPosition, Color.magenta, true);
            }

            if (cell.tower != null)
            {
                ColorCell(cell.cellPosition, Color.yellow, true);
                cellsWithTowers.Add(cell);
            }
        }
        return cellsWithTowers;
    }

}
