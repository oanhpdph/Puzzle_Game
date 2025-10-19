using System.Collections.Generic;
using UnityEngine;

public class BoardController : Singleton<BoardController>
{
    public int[,] board;
    public UI_Score UI_Score;
    private List<int> colFull = new();
    private List<int> rowFull = new();

    private float multiplier = 0.5f;

    public float score = 0;
    private Vector3 positionScore;
    public int combo = 0;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        board = new int[9, 9];

        List<CellWrapper> cellData = GameController.Instance.AllCell?.listRow;
        if (cellData != null && cellData.Count > 0)
        {
            for (int i = 0; i < cellData.Count; i++)
            {
                for (int j = 0; j < cellData[i].cell.Count; j++)
                {
                    List<CellData> cell = cellData[i].cell;
                    board[i, j] = cell[j].status;
                }
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[i, j] = 0;
                }
            }
        }
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
                positionScore = CellGenerator.Instance.cells[row, 9 / 2].transform.position;
                Cell cell = CellGenerator.Instance.cells[row, i].GetComponent<Cell>();
                cell.ResetCell();

                board[row, i] = 0;
            }
            foreach (var col in colFull)//clear col
            {
                if (board[i, col] == 0)
                    continue;
                positionScore = CellGenerator.Instance.cells[9 / 2, col].transform.position;
                Cell cell = CellGenerator.Instance.cells[i, col].GetComponent<Cell>();
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
                score *= combo;
            }
            UI_Score.AddScore(score, positionScore, combo);
            score = 0;
            multiplier = 0.5f;
        }
    }
    private void OnApplicationQuit()
    {
        SaveController.SaveData();
    }
}