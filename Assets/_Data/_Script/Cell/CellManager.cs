using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class CellManager : MonoBehaviour
{
    public static int[,] cells;
    public static GameObject[,] _Cells;
    public GameObject cellPrefab;

    public Vector2 size = new(9, 9);
    private Vector2 startPosition = new(-400, 400);

    private CellData[] cellData;
    private void Awake()
    {
        LoadCell();
        GeneratorCell();
    }

    private void LoadCell()
    {
        Cells cells = SaveController.LoadData<Cells>("CellData.json");
        if (cells != null)
        {
            cellData = cells.arrData;
            return;
        }
    }

    public void GeneratorCell()
    {
        cells = new int[9, 9];
        _Cells = new GameObject[9, 9];

        int k = 0;
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                cells[i, j] = 0;
                GameObject cellSpawn = Instantiate(cellPrefab, transform.position, quaternion.identity, transform);
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = GetPosition(j, i);
                _Cells[i, j] = cellSpawn;

                if (cellData != null)
                {

                    Sprite sprite = SpriteController.Instance.GetSpriteBlock(cellData[k].spriteName);
                    if (sprite != null)
                    {
                        Image image = cellSpawn.GetComponent<Image>();
                        image.sprite = sprite;
                        image.pixelsPerUnitMultiplier = 100f;
                        image.color = Color.white;
                    }
                    cellSpawn.GetComponent<Cell>().Status = cellData[k].status;
                    cells[i, j] = cellData[k].status == 1 ? 1 : 0;
                }
                k++;
            }
        }
    }

    public Vector2 GetPosition(int i, int j)
    {
        Vector2 position = new(startPosition.x + (i * 100), startPosition.y + (j * -100));
        return position;
    }

    public static Cells GetAllCell()
    {
        CellData[] cellData = new CellData[81];
        int index = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                CellData cell = new()
                {
                    spriteName = _Cells[row, col].GetComponent<Image>().sprite.name,
                    status = _Cells[row, col].GetComponent<Cell>().Status,
                };
                cellData[index] = cell;
                index++;
            }
        }

        return new Cells() { arrData = cellData };
    }

    //public int CountBlankCells()
    //{
    //    int count = 0;
    //    for (int i = 0; i < size.y; i++)
    //    {
    //        for (int j = 0; j < size.x; j++)
    //        {
    //            if (cells[i, j] == 0)
    //            {
    //                count++;
    //            }
    //        }
    //    }
    //    return count;
    //}





    public void TestCell(int[,] test)
    {
        string result = "";

        for (int i = 0; i < test.GetLength(0); i++)//x
        {
            for (int j = 0; j < test.GetLength(1); j++)//y
            {
                result += test[i, j] + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }
}
