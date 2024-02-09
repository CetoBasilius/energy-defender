using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameplay
{
    [Serializable]
    public class GridCell
    {
        public char tileChar;
        public TileBase tileBase;
        public Vector3Int cellPosition;

        public Tower tower;
        public List<Tower> towersWatching = new List<Tower>();
        public List<Enemy> enemies = new List<Enemy>();
        
        public GridCell nextPathCell;
        public GridCell previousPathCell;

        private Vector3 position;

        public GridCell(char tileChar, TileBase tileBase, Vector3Int cellPosition)
        {
            this.tileChar = tileChar;
            this.tileBase = tileBase;
            this.cellPosition = cellPosition;
            this.position = new Vector3(cellPosition.x + 0.5f, cellPosition.y + 0.5f, 0);
        }

        public bool isPath()
        {
            return this.tileChar == 'P';
        }

        public bool isAvailable()
        {
            bool hasTower = this.tower != null;
            bool isPath = this.isPath();
            bool isBlank = this.tileChar == 'B';

            return !hasTower && !isPath && isBlank;
        }

        public GridCell GetFirstPrevious()
        {
            GridCell currentCell = this;
            while (currentCell.previousPathCell != null)
            {
                currentCell = currentCell.previousPathCell;
            }
            return currentCell;
        }

        public Vector3 GetPosition()
        {
            return this.position;
        }

        internal bool NotifyEnemyEntered(Enemy enemy)
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }

            bool notified = false;
            foreach (Tower tower in towersWatching)
            {
                notified = tower.AddTarget(this) || notified;
            }
            return notified;
        }

        internal bool NotifyEnemyLeft(Enemy enemy)
        {
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
            }
            
            bool notified = false;
            foreach (Tower tower in towersWatching)
            {
                notified = tower.RemoveTarget(this) || notified;
            }
            return notified;
        }
    }
}
