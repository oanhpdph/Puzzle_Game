using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataShape
{
    public List<BlockData> blocks;
}
[Serializable]
public class BlockData
{
    public int indexOfShape = -1;
    public string sprite;
    public bool active;
    public int id;
    public BlockData(int block, string sprite, int id, bool active)
    {
        this.indexOfShape = block;
        this.sprite = sprite;
        this.id = id;
        this.active = active;
    }
}
[Serializable]
public class BlockShape
{
    public Vector2Int[] cells;
    public BlockShape(Vector2Int[] cells)
    {
        this.cells = cells;
    }
}

