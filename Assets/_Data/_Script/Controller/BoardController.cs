using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private static BoardController instance { get; set; }
    public static BoardController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BoardController>();
            }
            return instance;
        }
    }
    public GameObject[,] cells;
    public int[,] board;
    public UI_Score UI_Score;
    private List<int> colFull = new();
    private List<int> rowFull = new();

    private float multiplier = 0.5f;

    public float score = 0;
    private Vector3 positionScore;
    private int combo = 0;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
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
        CheckCombo();
        Clear();
        CalculateScore();
    }

    public void FillRowCol(int row, int col)
    {
        int counterRow = 0;
        int counterCol = 0;
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == 1)
                counterRow++;
            if (board[i, col] == 1)
                counterCol++;
        }

        if (counterRow == 9)
            rowFull.Add(row);
        if (counterCol == 9)
            colFull.Add(col);
    }
    private void CheckCombo()
    {
        if (colFull.Count != 0 || rowFull.Count != 0)
        {
            combo++;
        }
        else
        {
            combo = 0;
        }
    }
    private void Clear()
    {
        if (rowFull.Count == 0 && colFull.Count == 0) return;

        for (int i = 0; i < 9; i++)
        {
            foreach (var row in rowFull)//clear row
            {
                if (board[row, i] == 0)
                    continue;
                positionScore = cells[row, 9 / 2].transform.position;
                Cell cell = cells[row, i].GetComponent<Cell>();
                cell.ResetCell();

                board[row, i] = 0;
            }
            foreach (var col in colFull)//clear col
            {
                if (board[i, col] == 0)
                    continue;
                positionScore = cells[9 / 2, col].transform.position;
                Cell cell = cells[i, col].GetComponent<Cell>();
                cell.ResetCell();

                board[i, col] = 0;
            }
        }
        score += 10 * (rowFull.Count + colFull.Count);
        multiplier += 0.5f * (rowFull.Count + colFull.Count);
        rowFull.Clear();
        colFull.Clear();
    }
    private void CalculateScore()
    {
        if (multiplier > 0.5f)
        {
            score *= multiplier;
            if (combo >= 2)
            {
                score *= 2;
            }
            UI_Score.AddScore(score, positionScore, combo);
            score = 0;
            multiplier = 0.5f;
        }
    }
}