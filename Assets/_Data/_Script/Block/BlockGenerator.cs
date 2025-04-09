using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockGenerator : MonoBehaviour
{
    private List<int[,]> allBlock;
    public GameObject defaultBlock;

    public GameObject image;
    public CellManager cellManager;
    public Dictionary<GameObject, int[,]> AllCurrentBlock = new();
    public UI_EndGame UI_EndGame;
    private void Start()
    {
        Initalized();
        LoadBlock();

    }

    private void Update()
    {
        if (AllCurrentBlock.Count == 0)
        {

            GeneratorBlock();
        }
    }
    public void GeneratorBlock()
    {
        while (AllCurrentBlock.Count < 3)
        {
            int index = Random.Range(0, SatisfyBlock().Count);
            int[,] block = allBlock[index];
            DisplayBlock(block, null, AllCurrentBlock.Count);
        }
    }

    //private void CalculateBlock()
    //{
    //    listSatisfy = new();
    //    for (int i = 0; i < allBlock.Count; i++)
    //    {
    //        for (int row = 0; row < cellManager.cells.GetLength(0); row++)//x
    //        {
    //            for (int col = 0; col < cellManager.cells.GetLength(1); col++)//y
    //            {
    //                int cellValue = cellManager.cells[row, col];

    //                int[,] block = allBlock[i];
    //                bool isSkip = false;
    //                for (int rowBlock = 0; rowBlock < block.GetLength(0); rowBlock++)
    //                {
    //                    for (int colBlock = 0; colBlock < block.GetLength(0); colBlock++)
    //                    {
    //                        if (cellValue == 1)
    //                        {
    //                            if (block[rowBlock, colBlock] == 1)
    //                            {
    //                                isSkip = true;
    //                                break;
    //                            }
    //                        }
    //                    }
    //                    if (isSkip) break;
    //                }
    //                if (!isSkip)
    //                    listSatisfy.Add(block);
    //            }
    //        }
    //    }
    //}
    private List<int[,]> SatisfyBlock()
    {
        List<int[,]> satisfyList = new();
        for (int i = 0; i < allBlock.Count; i++)
        {
            if (CanPlace(CellManager.cells, allBlock[i]))
            {
                satisfyList.Add(allBlock[i]);
            }
        }
        return satisfyList;
    }
    private void LoadBlock()
    {
        Blocks blockData = SaveController.LoadData<Blocks>("BlockData.json");
        if (blockData.blocks != null)
        {
            for (int i = 0; i < blockData.blocks.Length; i++)
            {
                if (blockData.blocks[i].block.Length > 0)
                {
                    int[,] block = StringToArrayInt2D(blockData.blocks[i].block);
                    DisplayBlock(block, blockData.blocks[i].sprite, i);
                }
            }
            if (!CheckPlace(AllCurrentBlock.Values.ToList()))
            {
                UI_EndGame.EndGame();
            }
        }
        else
        {
            GeneratorBlock();
        }

    }
    private int[,] StringToArrayInt2D(string data)
    {
        string[] rowBlock = data.Split("/");// ex 10/10/01/

        int[,] block = new int[rowBlock.Length, rowBlock[0].Length];

        for (int row = 0; row < rowBlock.Length; row++)// ex: 10
        {
            int[] column = rowBlock[row].Select(x => x - '0').ToArray();

            for (int col = 0; col < column.Length; col++)
            {
                block[row, col] = column[col];
            }
        }
        return block;
    }

    private GameObject DisplayBlock(int[,] objGenerator, string spriteName, int index)
    {
        Sprite sprite = GetSprite(spriteName);

        GameObject parrentObj = InstantiateParent(defaultBlock, transform);

        for (int row = 0; row < objGenerator.GetLength(0); row++)// x
        {
            for (int col = 0; col < objGenerator.GetLength(1); col++)//y
            {
                if (objGenerator[row, col] == 1)
                {
                    GameObject generator = Instantiate(image, parrentObj.transform);
                    generator.GetComponent<RectTransform>().anchoredPosition = new Vector2(100 * col, -100 * row);
                    generator.GetComponent<Image>().sprite = sprite;
                }
            }
        }

        float x = objGenerator.GetLength(0);
        float y = objGenerator.GetLength(1);

        y = y == 1 || y == 2 ? (y - 1) :
                y == 3 || y == 4 ? (y + 1) :
                y + 2;
        x = x == 1 || x == 2 ? (x - 1) :
                x == 3 || x == 4 ? (x + 1) :
                x + 2;

        parrentObj.transform.localPosition = new(-25 * Mathf.CeilToInt((y) / 2), 300 - (index * 300) + 25 * Mathf.CeilToInt(x / 2));

        AllCurrentBlock.Add(parrentObj, objGenerator);
        return gameObject;
    }
    private Sprite GetSprite(string spriteName)
    {
        if (spriteName == null)
        {
            return SpriteController.Instance.GetRandomSpriteBlock();
        }
        else
        {
            return SpriteController.Instance.GetSpriteBlock(spriteName);
        }
    }
    private GameObject InstantiateParent(GameObject patternObj, Transform transformParent)
    {
        GameObject parrentObj = Instantiate(patternObj, transformParent);
        parrentObj.transform.SetParent(transform);
        parrentObj.transform.localScale = new(0.5f, 0.5f, 0.5f);
        parrentObj.name = AllCurrentBlock.Count.ToString();
        MultipleDrag multipleDrag = parrentObj.GetComponent<MultipleDrag>();
        multipleDrag.AllCurrentBlock = AllCurrentBlock;
        return parrentObj;
    }

    public Blocks GetBlockData()
    {
        BlockData[] blockData = new BlockData[3];

        foreach (var item in AllCurrentBlock)
        {
            int index = int.Parse(item.Key.name);

            Sprite sprite = item.Key.transform.GetChild(0).GetComponent<Image>().sprite;
            blockData[index] = new()
            {
                sprite = sprite.name,
                block = Array2DToString(item.Value),
            };
        }
        return new Blocks() { blocks = blockData };
    }

    private string Array2DToString<T>(T[,] arr2D)
    {
        string data = "";

        int row = arr2D.GetLength(0);
        int col = arr2D.GetLength(1);
        for (int i = 0; i < row; i++)// x
        {
            for (int j = 0; j < col; j++)//y
            {
                data += arr2D[i, j].ToString();
            }
            if (row != i + 1)
            {
                data += "/";
            }
        }
        return data;
    }

    private bool CanPlace(int[,] board, int[,] block)
    {
        for (int row = 0; row <= board.GetLength(0) - block.GetLength(0); row++)
        {
            for (int col = 0; col <= board.GetLength(1) - block.GetLength(1); col++)
            {
                Debug.Log(row + " " + col);
                if (CanPlaceAt(board, block, row, col))
                    return true;

            }
        }
        return false;
    }

    private bool CanPlaceAt(int[,] board, int[,] block, int startRow, int startCol)
    {
        for (int _row = 0; _row < block.GetLength(0); _row++)
        {
            for (int _col = 0; _col < block.GetLength(1); _col++)
            {
                if (block[_row, _col] == 1 && board[startRow + _row, startCol + _col] != 0)
                    return false;
            }
        }
        return true;
    }

    public bool CheckPlace(List<int[,]> listCheck)
    {
        bool result = false;

        if (listCheck.Count == 0)
        {
            return true;
        }
        for (int i = 0; i < listCheck.Count; i++)
        {
            if (CanPlace(CellManager.cells, listCheck[i]))
            {
                result = true;
                break;
            }
            result = false;
        }
        return result;
    }
    private void Initalized()
    {
        allBlock = new List<int[,]>()
        {
            new int[,]{ { 1 } },//1

            new int[,]{ { 1, 1 } },//2
            new int[,]{ { 1 },//3
                        { 1 } },

            new int[,]{ { 1 },//4
                        { 1 },
                        { 1 } },
            new int[,]{ { 1, 1, 1 } },//5

            new int[,]{ { 1 , 0 },//6
                        { 0 , 1 } },
            new int[,]{ { 0 , 1 },//7
                        { 1 , 0 } },

            new int[,]{ { 1 , 1 },//8
                        { 1 , 0 } },
            new int[,]{ { 1 , 1 },//9
                        { 0 , 1 } },
            new int[,]{ { 1 , 0 },//10
                        { 1 , 1 } },
            new int[,]{ { 0 , 1 },
                        { 1 , 1 } },

            new int[,]{ { 1 , 1 },
                        { 1 , 1 } },

            new int[,]{ { 1 , 1 , 1 },
                        { 0 , 0 , 1 } },
            new int[,]{ { 1 , 1 , 1 },
                        { 1 , 0 , 0 } },
            new int[,]{ { 1 , 0 , 0 },
                        { 1 , 1 , 1 } },
            new int[,]{ { 0 , 0 , 1 },
                        { 1 , 1 , 1 } },

            new int[,]{ { 0 , 1 , 1 },
                        { 1 , 1 , 0 } },
            new int[,]{ { 1 , 1 , 0 },
                        { 0 , 1 , 1 } },
            new int[,]{ { 1 , 0 },
                        { 1 , 1 },
                        { 0 , 1 } },
            new int[,]{ { 0 , 1 },
                        { 1 , 1 },
                        { 1 , 0 } },

            new int[,]{ { 0 , 1 },
                        { 0 , 1 },
                        { 1 , 0 } },
            new int[,]{ { 0 , 1 },
                        { 1 , 0 },
                        { 1 , 0 } },
            new int[,]{ { 1 , 0 },
                        { 1 , 0 },
                        { 0 , 1 } },
            new int[,]{ { 1 , 0 },
                        { 0 , 1 },
                        { 0 , 1 } },

            new int[,]{ { 1 , 1 , 1 },
                        { 1 , 1 , 1 } },
            new int[,]{ { 1 , 1 },
                        { 1 , 1 },
                        { 1 , 1 } },

            new int[,]{ { 0 , 0 , 1 },
                        { 0 , 0 , 1 },
                        { 1 , 1 , 1 } },
            new int[,]{ { 1 , 1 , 1 },
                        { 1 , 0 , 0 },
                        { 1 , 0 , 0 } },
            new int[,]{ { 1 , 0 , 0 },
                        { 1 , 0 , 0 },
                        { 1 , 1 , 1 } },
            new int[,]{ { 1 , 1 , 1 },
                        { 0 , 0 , 1 },
                        { 0 , 0 , 1 } },

            new int[,]{ { 0 , 0 , 1 },
                        { 0 , 1 , 0 },
                        { 1 , 0 , 0 } },
            new int[,]{ { 1 , 0 , 0 },
                        { 0 , 1 , 0 },
                        { 0 , 0 , 1 } },

            new int[,]{ { 1 , 1 , 1 },
                        { 1 , 1 , 1 },
                        { 1 , 1 , 1 } },
            new int[,]{ { 1 , 1 , 1 , 1, 1 }},
            new int[,]{ { 1 },
                        { 1 },
                        { 1 },
                        { 1 },
                        { 1 } }
        };

    }

}
