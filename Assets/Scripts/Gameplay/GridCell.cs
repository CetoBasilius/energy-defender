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

        public GridCell(char tileChar, TileBase tileBase, Vector3Int cellPosition)
        {
            this.tileChar = tileChar;
            this.tileBase = tileBase;
            this.cellPosition = cellPosition;
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
    }
}
