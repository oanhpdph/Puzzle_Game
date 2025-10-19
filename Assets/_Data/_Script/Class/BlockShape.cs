using System;
using UnityEngine;

[Serializable]
public class BlockShape
{
    public Vector2Int[] cells;
    public int maxAdjacentCell;
    public float percentAdjacent;
    public BlockShape(Vector2Int[] cells, int maxAdjacentCell = 0)
    {
        this.cells = cells;
        this.maxAdjacentCell = maxAdjacentCell;
    }
}