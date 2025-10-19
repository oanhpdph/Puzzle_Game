using System;
using UnityEngine;

public class GameController : PersistentSingleton<GameController>
{
    [SerializeField] private SpriteConfig spriteConfig;
    private ScoreData scoreData;
    private SaveDataShape saveDataShape;
    public delegate void OnDropShape();
    public event OnDropShape onDropShape;
    private AllCell allCell;
    private UndoData undoData;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        HUDSystem.Instance.Show<LoadingPanel>().StartLoading();
    }
    public void DropShapeAction()
    {
        onDropShape?.Invoke();
    }
    public void ReLoadData()
    {
        scoreData = SaveLoadExt.LoadData<ScoreData>(Flags.SCORE_DATA_FILE);
        undoData = SaveLoadExt.LoadData<UndoData>(Flags.UNDO_DATA_FILE);
        allCell = SaveLoadExt.LoadData<AllCell>(Flags.CELL_DATA_FILE);
        saveDataShape = SaveLoadExt.LoadData<SaveDataShape>(Flags.BLOCK_DATA_FILE);
    }
    public SaveDataShape SaveDataShape
    {
        get
        {
            return saveDataShape;
        }
        set
        {
            saveDataShape = value;
        }
    }
    public ScoreData ScoreData
    {
        get
        {
            return scoreData;
        }
        set
        {
            scoreData = value;
        }
    }
    public SpriteConfig SpriteConfig
    {
        get
        {
            return spriteConfig;
        }
        set
        {
            spriteConfig = value;
        }
    }
    public UndoData UndoData
    {
        get
        {
            return undoData;
        }
        set
        {
            undoData = value;
        }
    }
    public AllCell AllCell
    {
        get
        {
            return allCell;
        }
        set
        {
            allCell = value;
        }
    }
}
