using Assets._Data._Script.Class;
using Assets._Data._Script.Controller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    private static BoardController instance { get; set; }
    public static BoardController Instance => instance;


    [SerializeField] private GameObject prefabCell;
    [SerializeField] private TextMeshProUGUI goalScoreUGUI;
    [SerializeField] private TextMeshProUGUI currentScoreUGUI;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject highScoreNoti;

    public Vector2 size = new(9, 9); //x is col, y is row
    public GameObject[,] arrayCell;
    private SaveData[] arrSaveData;
    private List<int> colFull;
    private List<int> rowFull;

    private Image imagePrefab;
    private float multiplier = 0.5f;

    private float currentScore = 0;
    public float score = 0;
    //private float totalScore = 0;
    private float goalScore;
    private bool isHigh;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isHigh = PlayerPrefs.GetInt("isHigh") == 1;
        goalScore = PlayerPrefs.GetFloat("goalScore");
        goalScoreUGUI.text = goalScore.ToString();

        currentScore = PlayerPrefs.GetFloat("currentScore");
        currentScoreUGUI.text = currentScore.ToString();
        if (goalScore == 0)
        {
            isHigh = true;
        }

        arrayCell = new GameObject[(int)size.x, (int)size.y];
        scoreText.SetActive(false);
        LoadCell();
        AddCell();
    }
    private void Start()
    {
        colFull = new List<int>();
        rowFull = new List<int>();
        imagePrefab = prefabCell.GetComponent<Image>();
    }

    private void AddCell()
    {
        int k = 0;
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                GameObject cell = transform.GetChild(k).gameObject;
                arrayCell[j, i] = cell;
                if (arrSaveData != null)
                {
                    Sprite sprite = SpriteController.Instance.GetSpriteBlock(arrSaveData[k].spriteName);
                    if (sprite != null)
                    {
                        Image image = cell.GetComponent<Image>();
                        image.sprite = sprite;
                        image.pixelsPerUnitMultiplier = 100f;
                        image.color = Color.white;
                    }
                    cell.GetComponent<Cell>().Status = arrSaveData[k].status;
                }
                k++;
            }
        }
    }

    public void CheckFull()
    {
        LoopColumn();
        LoopRow();
        ClearRowAndColFull();
    }
    private void LoopRow()
    {
        for (int row = 0; row < size.y; row++)
        {
            int counter = 0;
            for (int column = 0; column < size.x; column++)
            {
                if (arrayCell[column, row].GetComponent<Cell>().Status == true)
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
                if (arrayCell[col, row].GetComponent<Cell>().Status == true)
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
            scoreText.transform.position = arrayCell[col, (int)size.y / 2].transform.position;

            for (int row = 0; row < size.y; row++)
            {
                //int index = (int)(col + row * size.x);

                GameObject cell = arrayCell[col, row];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = imagePrefab.sprite;
                imageCell.color = imagePrefab.color;
                imageCell.pixelsPerUnitMultiplier = imagePrefab.pixelsPerUnitMultiplier;
                imageCell.raycastPadding = imagePrefab.raycastPadding;

                cell.GetComponent<Cell>().Status = false;
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
            scoreText.transform.position = arrayCell[(int)size.x / 2, row].transform.position;

            for (int col = 0; col < size.y; col++)
            {
                //int index = (int)(col + row * size.y);

                GameObject cell = arrayCell[col, row];
                Image imageCell = cell.GetComponent<Image>();

                imageCell.sprite = imagePrefab.sprite;
                imageCell.color = imagePrefab.color;
                imageCell.pixelsPerUnitMultiplier = imagePrefab.pixelsPerUnitMultiplier;
                imageCell.raycastPadding = imagePrefab.raycastPadding;

                cell.GetComponent<Cell>().Status = false;
            }
            multiplier += 0.5f;
            score += 10;
        }
        rowFull.Clear();
    }
    public bool CheckEndGame(GameObject reserveObject)
    {
        bool isEnd = true;// end game

        int maxCol = arrayCell.GetLength(1);
        int maxRow = arrayCell.GetLength(0);

        for (int col = 0; col < maxCol; col++)// col
        {
            for (int row = 0; row < maxRow; row++)// row
            {
                if (arrayCell[col, row].GetComponent<Cell>().Status == false)
                {
                    if (reserveObject.transform.childCount == 0)
                    {
                        isEnd = false;
                        return isEnd;
                    }

                    foreach (Transform shape in reserveObject.transform)// shape detail
                    {
                        RectTransform rectTransform = shape.GetComponent<RectTransform>();

                        int colChildren = col;
                        int rowChildren = row;
                        bool isBreak = false;

                        colChildren += (int)rectTransform.anchoredPosition.x / 100;
                        rowChildren -= (int)rectTransform.anchoredPosition.y / 100;

                        if (rowChildren < 0)
                        {
                            isBreak = true;
                        }
                        if (colChildren < 0)
                        {
                            col -= colChildren;
                            isBreak = true;
                        }

                        if (rowChildren > maxRow - 1)
                        {
                            maxRow -= (rowChildren - maxRow);
                            isBreak = true;
                        }
                        if (colChildren > maxCol - 1)
                        {
                            maxCol -= (colChildren - maxCol);
                            isBreak = true;
                        }

                        if (isBreak || arrayCell[colChildren, rowChildren].GetComponent<Cell>().Status == true)
                        {
                            isEnd = true;
                            break;// stop loop
                        }
                        else
                            isEnd = false;

                    }

                    if (!isEnd)
                        return isEnd;// is end = false = continue game
                }

            }
        }
        return isEnd;// is end = true = end game
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
            scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
            AddScore(score);
            scoreText.SetActive(true);
            StartCoroutine(DelayScore());
            score = 0;
            multiplier = 0.5f;
        }
    }
    public void AddScore(float score)
    {
        currentScore += score;
        PlayerPrefs.SetFloat("currentScore", currentScore);
        if (currentScore > goalScore)
        {
            goalScoreUGUI.text = currentScore.ToString();
            PlayerPrefs.SetFloat("goalScore", currentScore);

            if (!isHigh)
            {
                StartCoroutine(ShowNotification());
                isHigh = true;
                PlayerPrefs.SetInt("isHigh", 1);
            }
        }
        currentScoreUGUI.text = currentScore.ToString();
    }
    private IEnumerator DelayScore()
    {
        yield return new WaitForSeconds(0.5f);
        scoreText.SetActive(false);
    }

    private void LoadCell()
    {
        arrSaveData = SaveController.Instance.LoadCell();


    }

    private IEnumerator ShowNotification()
    {
        highScoreNoti.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        highScoreNoti.SetActive(false);

    }
}
