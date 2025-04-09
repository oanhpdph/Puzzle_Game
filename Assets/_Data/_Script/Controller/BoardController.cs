using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    private static BoardController instance { get; set; }
    public static BoardController Instance => instance;

    public Vector2 size = new(9, 9); //x is col, y is row
    public GameObject[,] arrayCell;
    public UI_Score UI_Score;
    public BlockGenerator blockGenerator;
    private List<int> colFull;
    private List<int> rowFull;

    private float multiplier = 0.5f;

    public float score = 0;
    public UI_EndGame UI_EndGame;

    private Vector3 positionScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        colFull = new List<int>();
        rowFull = new List<int>();
        arrayCell = CellManager._Cells;
    }
    public void CheckFull()
    {
        LoopColumn();
        LoopRow();
        ClearRowAndColFull();

        if (!blockGenerator.CheckPlace(blockGenerator.AllCurrentBlock.Values.ToList()))
        {
            UI_EndGame.EndGame();
        };

    }
    private void LoopRow()
    {
        for (int row = 0; row < size.y; row++)
        {
            int counter = 0;
            for (int column = 0; column < size.x; column++)
            {
                if (arrayCell[column, row].GetComponent<Cell>().Status == 1)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }

            if (counter == size.x)
            {
                rowFull.Add(row);
            }
        }
    }
    private void LoopColumn()
    {
        for (int col = 0; col < size.y; col++)
        {
            int counterCol = 0;

            for (int row = 0; row < size.y; row++)
            {
                if (arrayCell[col, row].GetComponent<Cell>().Status == 1)
                {
                    counterCol++;
                }
                else
                {
                    break;
                }
            }

            if (counterCol == size.y)
            {
                colFull.Add(col);
            }
        }
    }
    private void ClearRowAndColFull()
    {
        ClearColFull();
        ClearRowFull();
        StartCoroutine(Delay());
    }
    private void ClearColFull()
    {
        if (colFull.Count == 0) return;

        foreach (int col in colFull)
        {
            positionScore = arrayCell[col, (int)size.y / 2].transform.position;

            for (int row = 0; row < size.y; row++)
            {
                //int index = (int)(col + row * size.x);

                GameObject cell = arrayCell[col, row];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = SpriteController.Instance.GetSpriteDefault();
                imageCell.color = new(0.9f, 0.65f, 0.4f, 1);
                imageCell.pixelsPerUnitMultiplier = 1;

                cell.GetComponent<Cell>().Status = 0;
                CellManager.cells[col, row] = 0;
            }
            multiplier += 0.5f;
            score += 10;
        }
        colFull.Clear();
    }
    private void ClearRowFull()
    {
        if (rowFull.Count == 0) return;

        foreach (int row in rowFull)
        {
            positionScore = arrayCell[(int)size.x / 2, row].transform.position;

            for (int col = 0; col < size.y; col++)
            {
                //int index = (int)(col + row * size.y);

                GameObject cell = arrayCell[col, row];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = SpriteController.Instance.GetSpriteDefault();
                imageCell.color = new(0.9f, 0.65f, 0.4f, 1);
                imageCell.pixelsPerUnitMultiplier = 1;

                cell.GetComponent<Cell>().Status = 0;
                CellManager.cells[col, row] = 0;
            }
            multiplier += 0.5f;
            score += 10;
        }
        rowFull.Clear();
    }

    private IEnumerator Delay()
    {
        yield return new WaitUntil(() => colFull.Count == 0 && rowFull.Count == 0);
        CalculateScore();
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