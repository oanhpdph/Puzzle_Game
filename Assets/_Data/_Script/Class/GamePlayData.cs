using System;
using System.Collections.Generic;
/// <summary>
/// Data to undo function
/// gamePlayDatas = List<GamePlayData>
/// </summary>
[Serializable]
public class UndoData
{
    public List<GamePlayData> gamePlayDatas;
    public int maxUndo = 3;
}
/// <summary>
/// listRow = List<CellWrapper>
/// </summary>
[System.Serializable]
public class AllCell
{
    public List<CellWrapper> listRow;
}

/// <summary>
/// Game Play Data  
/// listRow =  List<CellWrapper>
/// blocks = List<BlockData>
/// scoreData = ScoreData
/// </summary>
[Serializable]
public class GamePlayData
{
    public List<CellWrapper> listRow;
    public List<BlockData> blocks;
    public ScoreData scoreData;
    public int combo;
}

/// <summary>
/// cell = List<CellData>
/// </summary>
[Serializable]
public class CellWrapper
{
    public List<CellData> cell;
}
/// <summary>
/// spriteName = string
/// status = int(1 = true, 0 = false)
/// </summary>
[Serializable]
public class CellData
{
    public string spriteName;
    public int status;
    public CellData() { }
}