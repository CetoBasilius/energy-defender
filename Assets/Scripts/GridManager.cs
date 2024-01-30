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

    private GridCell[,] gridCells;
    private List<GridCell> verticalPathCells = new List<GridCell>();
    private List<GridCell> cornerPathCells = new List<GridCell>();

    private int tilemapWidth;
    private int tilemapHeight;
    private int tilemapStartX;
    private int tilemapStartY;

    private GridCell baseCell;
    private GridCell enemySpawn;

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
            GridCell lastPathCell = null;
            string line = tiledata[rowIndex];
            bool baseCornerPath = false;
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

                    if (gridCell.isPath())
                    {
                        if (lastPathCell != null) // New path cell and last path cell are connected
                        {
                            lastPathCell.previousPathCell = gridCell;
                            gridCell.nextPathCell = lastPathCell;
                        }
                        else // No previous path cell
                        {
                            if (charCol == 0)
                            {
                                baseCornerPath = true;
                                baseCell = gridCell;
                                ColorCell(gridCell.cellPosition, Color.green);
                            }
                            else
                            {
                                ColorCell(gridCell.cellPosition, Color.magenta);
                                cornerPathCells.Add(gridCell);
                            }
                        }
                        lastPathCell = gridCell;
                    }
                    else
                    {
                        if (lastPathCell != null)
                        {
                            if (cornerPathCells.Contains(lastPathCell)) // Is not corner
                            {
                                ColorCell(lastPathCell.cellPosition, Color.cyan);
                                cornerPathCells.Remove(lastPathCell);
                                verticalPathCells.Add(lastPathCell);
                            }
                            else
                            {
                                ColorCell(lastPathCell.cellPosition, Color.yellow);
                                if (baseCornerPath)
                                {
                                    baseCornerPath = false;
                                    cornerPathCells.Insert(0, lastPathCell);
                                }
                                else
                                {
                                    cornerPathCells.Add(lastPathCell);
                                }
                            }
                        }
                        lastPathCell = null;
                    }
                }
            }

            if (lastPathCell != null)
            {
                ColorCell(lastPathCell.cellPosition, Color.red);
                enemySpawn = lastPathCell;
            }
        }
    }

    private void CalculateEnemyPath()
    {
        Debug.Log("There are " + verticalPathCells.Count + " vertical path cells");
        Debug.Log("There are " + cornerPathCells.Count + " corner path cells");

        GridCell currentCorner = cornerPathCells[0];
        cornerPathCells.RemoveAt(0);
        FindPreviousVerticalCell(currentCorner);

        if (baseCell.GetFirstPrevious() == enemySpawn)
        {
            Debug.Log("Enemy path is valid");
        }
        else
        {
            Debug.LogError("Enemy path is invalid");
        }
    }

    private void FindPreviousVerticalCell(GridCell currentCell)
    {
        GridCell previousVerticalCell = null;
        foreach (GridCell cornerCell in cornerPathCells)
        {
            int yDiffAbs = Math.Abs(cornerCell.cellPosition.y - currentCell.cellPosition.y);
            int xDiff = cornerCell.cellPosition.x - currentCell.cellPosition.x;
            if (yDiffAbs == 1 && xDiff == 0) // currentCell is above or below cornerCell
            {
                // Set currentCell as nextPathCell of cornerCell, and cornerCell as previousPathCell of currentCell, swap the whole thing if cornerCell already has a nextPathCell
                if (cornerCell.nextPathCell != null)
                {
                    SwapLinkedCells(cornerCell);
                }
                cornerCell.nextPathCell = currentCell;
                currentCell.previousPathCell = cornerCell;

                previousVerticalCell = cornerCell;
                break;
            }
        }

        if (previousVerticalCell != null)
        {
            Debug.Log("Found previous corner cell");
            ColorCell(previousVerticalCell.cellPosition, Color.blue);
            // TODO: get next corner cell
            GridCell nextCorner = previousVerticalCell.GetFirstPrevious();
            ColorCell(nextCorner.cellPosition, new Color(1f, 0.5f, 0.5f, 1f));
            cornerPathCells.Remove(nextCorner);
            FindPreviousVerticalCell(nextCorner);
            return;
        }
        else
        {
            Debug.Log("No previous corner cell found");
        }

        foreach (GridCell verticalCell in verticalPathCells)
        {
            int yDiffAbs = Math.Abs(verticalCell.cellPosition.y - currentCell.cellPosition.y);
            int xDiff = verticalCell.cellPosition.x - currentCell.cellPosition.x;
            if (yDiffAbs == 1 && xDiff == 0) // currentCell is above or below verticalCell
            {
                // Set currentCell as nextPathCell of verticalCell, and verticalCell as previousPathCell of currentCell, swap the whole thing if verticalCell already has a nextPathCell
                if (verticalCell.nextPathCell != null)
                {
                    SwapLinkedCells(verticalCell);
                }
                verticalCell.nextPathCell = currentCell;
                currentCell.previousPathCell = verticalCell;

                previousVerticalCell = verticalCell;
                break;
            }
        }

        if (previousVerticalCell != null)
        {
            Debug.Log("Found previous vertical cell");
            ColorCell(previousVerticalCell.cellPosition, Color.blue);
            verticalPathCells.Remove(previousVerticalCell);
            FindPreviousVerticalCell(previousVerticalCell);
        }
        else
        {
            Debug.Log("No previous vertical cell found");
        }
    }

    private void SwapLinkedCells(GridCell swapCell)
    {
        GridCell currentNext = swapCell.nextPathCell;
        GridCell currentPrevious = swapCell.previousPathCell;

        if (swapCell.nextPathCell != null)
        {
            SwapLinkedCells(swapCell.nextPathCell);
        }

        swapCell.previousPathCell = currentNext;
        swapCell.nextPathCell = currentPrevious;
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
        enemy.transform.position = tilemap.GetCellCenterWorld(enemySpawn.cellPosition);
        enemy.transform.position.Set(enemy.transform.position.x, enemy.transform.position.y, -1);
        enemy.GetComponent<Enemy>().SetCurrentCell(enemySpawn);
    }
}
