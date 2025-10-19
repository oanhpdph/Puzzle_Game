using System.Collections.Generic;

public static class SaveController
{
    public static void SaveData()
    {
        SaveLoadExt.Save(Flags.BLOCK_DATA_FILE, BlockGenerator.Instance.GetBlockData());
        SaveLoadExt.Save(Flags.CELL_DATA_FILE, CellGenerator.Instance.GetAllCell());
        SaveLoadExt.Save(Flags.SCORE_DATA_FILE, GameController.Instance.ScoreData);
        SaveLoadExt.Save(Flags.UNDO_DATA_FILE, GameController.Instance.UndoData);
    }
    public static void ClearGameData()
    {

        SaveLoadExt.Save(Flags.BLOCK_DATA_FILE, new SaveDataShape());
        SaveLoadExt.Save(Flags.CELL_DATA_FILE, new AllCell());
        SaveLoadExt.Save(Flags.UNDO_DATA_FILE, new UndoData());
        SaveLoadExt.Save(Flags.SCORE_DATA_FILE, GameController.Instance.ScoreData.ResetData());

        GameController.Instance.ReLoadData();
    }
    public static void SaveUndoData()
    {
        SaveLoadExt.Save(Flags.UNDO_DATA_FILE, GameController.Instance.UndoData);
    }
    public static void AddUndoData()
    {
        ScoreData scoreData = new()
        {
            currentScore = GameController.Instance.ScoreData.currentScore,
            goalScore = GameController.Instance.ScoreData.goalScore,
            isHighScore = GameController.Instance.ScoreData.isHighScore,
            combo = 0
        };
        GamePlayData data = new()
        {
            listRow = CellGenerator.Instance.GetAllCell().listRow,
            blocks = BlockGenerator.Instance.GetBlockData().blocks,
            scoreData = scoreData,
        };
        UndoData undoData = GameController.Instance.UndoData;

        if (undoData == null)
        {
            undoData = new()
            {
                gamePlayDatas = new List<GamePlayData>(),
                maxUndo = HUDSystem.Instance.GetActivePanel<PlayPanel>().currentUndo
            };
            undoData.gamePlayDatas.Add(data);
        }
        else
        {
            undoData.gamePlayDatas ??= new List<GamePlayData>();
            undoData.gamePlayDatas.Add(data);
        }
        if (undoData.gamePlayDatas.Count > 5)
        {
            undoData.gamePlayDatas.RemoveAt(0);
        }
        GameController.Instance.UndoData = undoData;
    }
}
