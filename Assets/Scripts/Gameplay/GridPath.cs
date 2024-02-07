using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameplay
{
    public class GridPath
    {
        public List<GridCell> pathCells = new List<GridCell>();

        public GridCell baseCell;
        public GridCell enemySpawn;

        private List<GridCell> verticalPathCells = new List<GridCell>();
        private List<GridCell> cornerPathCells = new List<GridCell>();
        private GridCell lastPathCell;
        private bool baseCornerPath;

        public void ConnectHorizontalPathCells(GridCell gridCell, bool isFirst)
        {
            if (gridCell.isPath())
            {
                if (lastPathCell != null) // New path cell and last path cell are connected
                {
                    lastPathCell.previousPathCell = gridCell;
                    gridCell.nextPathCell = lastPathCell;
                }
                else // No previous path cell
                {
                    if (isFirst)
                    {
                        baseCornerPath = true;
                        baseCell = gridCell;
                    }
                    else
                    {
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
                        cornerPathCells.Remove(lastPathCell);
                        verticalPathCells.Add(lastPathCell);
                    }
                    else
                    {
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

        public bool IsValid()
        {
            if (baseCell == null || enemySpawn == null)
            {
                Debug.LogError("Enemy path is invalid, baseCell or enemySpawn is null");
                return false;
            }

            Debug.Log("There are " + verticalPathCells.Count + " vertical path cells");
            Debug.Log("There are " + cornerPathCells.Count + " corner path cells");

            if (baseCell.GetFirstPrevious() == enemySpawn)
            {
                Debug.Log("Enemy path is valid");
                return true;
            }

            Debug.LogError("Enemy path is invalid, baseCell is not connected to enemySpawn");
            return false;
        }

        public void ConnectVerticalPathCells(GridCell currentCell = null)
        {
            if (currentCell == null)
            {
                currentCell = cornerPathCells[0];
                cornerPathCells.RemoveAt(0);
            }

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
                // TODO: get next corner cell
                GridCell nextCorner = previousVerticalCell.GetFirstPrevious();
                cornerPathCells.Remove(nextCorner);
                ConnectVerticalPathCells(nextCorner);
                return;
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
                verticalPathCells.Remove(previousVerticalCell);
                ConnectVerticalPathCells(previousVerticalCell);
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

        public void NewRow()
        {
            lastPathCell = null;
            baseCornerPath = false;
        }

        public void EndRow()
        {
            if (lastPathCell != null)
            {
                enemySpawn = lastPathCell;
            }
        }
    }
}
