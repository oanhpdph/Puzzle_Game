using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CellGenerator : Singleton<CellGenerator>
{
    public GameObject cellPrefab;
    private Vector2 startPosition = new(50, -50);

    private List<CellWrapper> cellData;
    public GameObject[,] cells;
    private void Awake()
    {
        cells = new GameObject[9, 9];
    }
    private void Start()
    {
        GeneratorCell();
    }
    public void GeneratorCell()
    {
        cellData = GameController.Instance.AllCell?.listRow;
        int k = 0;
        int width = 9;
        int height = 9;
        bool hasData = false;
        if (cellData != null && cellData.Count > 0)
        {
            height = cellData.Count;
            width = cellData[0].cell.Count;
            hasData = true;
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject cellSpawn = Instantiate(cellPrefab, transform.position, quaternion.identity, transform);
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = GetPosition(i, j);

                Cell cellScript = cellSpawn.GetComponent<Cell>();
                cellScript.SetPosition(i, j);
                Sprite sprite;
                if (hasData)
                {
                    List<CellData> cell = cellData[i].cell;
                    sprite = GameController.Instance.SpriteConfig.GetSpriteBlock(cell[j].spriteName);
                    cellScript.Status = cell[j].status;
                }
                else
                {
                    sprite = GameController.Instance.SpriteConfig.GetSpriteDefault();
                    cellScript.Status = 0;
                }
                SetSprite(cellSpawn, sprite);
                cells[i, j] = cellSpawn;
                k++;
            }
        }
    }
    private void SetSprite(GameObject cellSpawn, Sprite sprite)
    {
        if (sprite != null)
        {
            Image image = cellSpawn.GetComponent<Image>();
            image.sprite = sprite;
            image.pixelsPerUnitMultiplier = 100f;
            image.color = Color.white;
        }
    }
    public Vector2 GetPosition(int i, int j)
    {
        Vector2 position = new(startPosition.x + (i * 100), startPosition.y + (j * -100));
        return position;
    }

    public AllCell GetAllCell()
    {
        List<CellWrapper> listRow = new();

        int index = 0;
        for (int row = 0; row < 9; row++)
        {
            List<CellData> cellData = new();

            for (int col = 0; col < 9; col++)
            {
                CellData cell;
                if (cells[row, col].GetComponent<Cell>().Status == 1)
                {
                    cell = new()
                    {
                        spriteName = cells[row, col].GetComponent<Image>().sprite.name,
                        status = cells[row, col].GetComponent<Cell>().Status,
                    };
                }
                else
                {
                    cell = new()
                    {
                        spriteName = GameController.Instance.SpriteConfig.GetSpriteDefault().name,
                        status = cells[row, col].GetComponent<Cell>().Status,
                    };
                }
                cellData.Add(cell);
                index++;
            }
            listRow.Add(new CellWrapper { cell = cellData });
        }

        return new AllCell() { listRow = listRow };
    }
    public void UndoCell()
    {
        Debug.Log(GameController.Instance.UndoData);
        cellData = GameController.Instance.UndoData?.gamePlayDatas[^1].listRow;
        int width = 9;
        int height = 9;
        bool hasData = false;
        if (cellData != null && cellData.Count > 0)
        {
            height = cellData.Count;
            width = cellData[0].cell.Count;
            hasData = true;
        }
        for (int i = 0; i < height; i++)
        {
            List<CellData> cell = cellData[i].cell;
            for (int j = 0; j < width; j++)
            {
                Cell cellScript = cells[i, j].GetComponent<Cell>();

                if (hasData)
                {
                    Sprite sprite = GameController.Instance.SpriteConfig.GetSpriteBlock(cell[j].spriteName);
                    if (sprite != null)
                    {
                        Image image = cells[i, j].GetComponent<Image>();
                        image.sprite = sprite;
                        image.pixelsPerUnitMultiplier = 100f;
                        image.color = Color.white;
                    }
                    cellScript.Status = cell[j].status;
                    BoardController.Instance.board[i, j] = cell[j].status;
                }
            }
        }
    }
}

