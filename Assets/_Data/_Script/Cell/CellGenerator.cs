using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class CellGenerator : MonoBehaviour
{
    public GameObject cellPrefab;

    private Vector2 startPosition = new(50, -50);

    private CellData[] cellData;

    private void Start()
    {
        GeneratorCell();
    }
    public void GeneratorCell()
    {
        cellData = BoardController.Instance.GetCellData();
        int k = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject cellSpawn = Instantiate(cellPrefab, transform.position, quaternion.identity, transform);
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = GetPosition(i, j);

                Cell cell = cellSpawn.GetComponent<Cell>();
                cell.SetPosition(i, j);
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

                    cell.Status = cellData[k].status;

                }
                BoardController.Instance.cells[i, j] = cellSpawn;
                k++;
            }
        }
    }

    public Vector2 GetPosition(int i, int j)
    {
        Vector2 position = new(startPosition.x + (i * 100), startPosition.y + (j * -100));
        return position;
    }

    public CellWrapper GetAllCell()
    {
        CellData[] cellData = new CellData[81];
        int index = 0;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                CellData cell = new()
                {
                    spriteName = BoardController.Instance.cells[row, col].GetComponent<Image>().sprite.name,
                    status = BoardController.Instance.cells[row, col].GetComponent<Cell>().Status,
                };
                cellData[index] = cell;
                index++;
            }
        }

        return new CellWrapper() { arrData = cellData };
    }
}
