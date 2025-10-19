using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : Panel
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button undoBtn;
    [SerializeField] private int maxUndo = 3;
    [SerializeField] private TextMeshProUGUI countTxt;
    public int currentUndo => maxUndo;
    private void Start()
    {
        pauseBtn.AddListener<object>(_ => PauseAction(), Listener.OnClick);
        undoBtn.AddListener<object>(_ => UndoAction(), Listener.OnClick);
    }
    private void OnEnable()
    {
        maxUndo = GameController.Instance.UndoData?.maxUndo ?? 3;
        countTxt.text = maxUndo.ToString();
    }
    private void PauseAction()
    {
        HUDSystem.Instance.Show<PausePanel>();
    }
    private void UndoAction()
    {
        if (maxUndo <= 0) return;
        UndoData undoData = GameController.Instance.UndoData;
        if (undoData == null || undoData.gamePlayDatas.Count == 0)
            return;
        GamePlayData gamePlay = undoData.gamePlayDatas[^1];
        GameController.Instance.SaveDataShape.blocks = gamePlay.blocks;
        GameController.Instance.AllCell.listRow = gamePlay.listRow;
        GameController.Instance.ScoreData = gamePlay.scoreData;
        BoardController.Instance.combo = 0;
        ReLoadUI();
        undoData.gamePlayDatas.RemoveAt(undoData.gamePlayDatas.Count - 1);
        GameController.Instance.UndoData = undoData;

    }
    private void ReLoadUI()
    {
        maxUndo--;
        countTxt.text = maxUndo.ToString();
        CellGenerator.Instance.UndoCell();
        BlockGenerator.Instance.LoadBlock();
        BoardController.Instance.UI_Score.DisplayScore();
    }
}
