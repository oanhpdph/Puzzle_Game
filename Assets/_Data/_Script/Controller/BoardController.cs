using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    private static BoardController instance { get; set; }
    public static BoardController Instance => instance;
    public GameObject[,] cells;
    public int[,] board;
    public UI_Score UI_Score;
    private List<int> colFull = new();
    private List<int> rowFull = new();

    private float multiplier = 0.5f;

    public float score = 0;
    //public UI_EndGame UI_EndGame;
    private Vector3 positionScore;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        board = new int[9, 9];
        cells = new GameObject[9, 9];

        CellData[] cellData = GetCellData();
        if (cellData != null)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[i, j] = cellData[9 * i + j].status;
                }
            }
        }
    }

    //private void OnEnable()
    //{
    //    GameController.Instance.onDropShape += CheckFull;

    //}
    //private void OnDisable()
    //{
    //    GameController.Instance.onDropShape -= CheckFull;

    //}
    public CellData[] GetCellData()
    {
        ISaveLoad saveLoad = new SaveLoad();
        CellWrapper cellWrapper = saveLoad.LoadData<CellWrapper>(Flags.CELL_DATA_FILE);
        if (cellWrapper != null)
        {
            return cellWrapper.arrData;
        }
        return default;
    }

    public void CheckFull()
    {
        ClearColFull();
        ClearRowFull();
        CalculateScore();
    }

    public void CheckRow(int row)
    {
        int counter = 0;
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == 1)
                counter++;
            else
                return;
        }
        rowFull.Add(row);
    }
    public void CheckCol(int col)
    {
        int counter = 0;

        for (int i = 0; i < 9; i++)
        {
            if (board[i, col] == 1)
                counter++;
            else
                return;
        }
        colFull.Add(col);
    }
    private void ClearColFull()
    {
        if (colFull.Count == 0) return;

        foreach (var col in colFull)
        {
            positionScore = cells[(int)9 / 2, col].transform.position;

            for (int row = 0; row < 9; row++)
            {
                GameObject cell = cells[row, col];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = SpriteController.Instance.GetSpriteDefault();
                imageCell.color = new(0.9f, 0.65f, 0.4f, 1);
                imageCell.pixelsPerUnitMultiplier = 1;

                cell.GetComponent<Cell>().Status = 0;
                board[row, col] = 0;
            }
            multiplier += 0.5f;
            score += 10;
        }
        colFull.Clear();
    }
    private void ClearRowFull()
    {
        if (rowFull.Count == 0) return;

        foreach (var row in rowFull)
        {
            positionScore = cells[row, (int)9 / 2].transform.position;

            for (int col = 0; col < 9; col++)
            {
                GameObject cell = cells[row, col];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = SpriteController.Instance.GetSpriteDefault();
                imageCell.color = new(0.9f, 0.65f, 0.4f, 1);
                imageCell.pixelsPerUnitMultiplier = 1;

                cell.GetComponent<Cell>().Status = 0;
                board[row, col] = 0;
            }
            multiplier += 0.5f;
            score += 10;
        }
        rowFull.Clear();
    }
    private void CalculateScore()
    {
        if (multiplier > 0.5f)
        {
            score *= multiplier;
            UI_Score.AddScore(score, positionScore);
            score = 0;
            multiplier = 0.5f;
        }
    }
}