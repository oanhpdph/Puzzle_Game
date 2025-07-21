using System;
using System.Collections.Generic;

[Serializable]
public class SaveDataShape
{
    public List<BlockData> blocks;
}
[Serializable]
public class BlockData
{
    public int id;
    public int indexOfShape = -1;
    public string sprite;
    public bool active;
    public BlockData(int block, string sprite, int id, bool active)
    {
        this.indexOfShape = block;
        this.sprite = sprite;
        this.id = id;
        this.active = active;
    }
}


