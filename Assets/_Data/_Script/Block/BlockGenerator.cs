using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockGenerator : MonoBehaviour
{
    private List<CurrentBlock> currentBlocks = new();
    public GameObject defaultBlock;

    public GameObject cell;
    public UI_EndGame UI_EndGame;
    private void Start()
    {
        LoadBlock();
    }
    private void OnEnable()
    {
        GameController.Instance.onDropShape += CounterShapeDone;

    }
    private void OnDisable()
    {
        GameController.Instance.onDropShape -= CounterShapeDone;

    }
    public void CounterShapeDone()
    {
        if (currentBlocks.Count(c => c.gameObject.activeSelf) == 0)
        {
            foreach (var item in currentBlocks)
            {
                Destroy(item.gameObject);
            }
            currentBlocks.Clear();
            GeneratorBlock();
        }
        CheckEndGame();
    }

    private void CheckEndGame()
    {
        if (currentBlocks.Count == 0)
            return;
        foreach (var item in currentBlocks)
        {
            if (item.gameObject.activeSelf)
            {
                if (CheckPlace(GetShape(item.indexInAllShape)))
                {
                    return;
                }
            }
        }

        GameController.Instance.CurrentState = StateGame.end;
    }
    public void GeneratorBlock()
    {
        List<int> priorityShapes = GetSmartPriorityShape();
        for (int i = 0; i < priorityShapes.Count; i++)
        {
            DisplayBlock(priorityShapes[i], null, currentBlocks.Count);
        }
    }

    private List<int> GetSmartPriorityShape()
    {
        List<(int index, int score)> scored = new();
        for (int i = 0; i < allShape.Count; i++)
        {
            int bestScore = GetClearScore(BoardController.Instance.board, allShape[i]);
            scored.Add((i, bestScore));
        }
        if (scored.Count == 0) return null;

        int maxScore = scored.Max(s => s.score);
        var sorted = scored.OrderByDescending(s => s.score).Select(s => s.index).ToList();

        int count = sorted.Count;
        int easyCount = Mathf.CeilToInt(count * 0.6f);

        List<int> candidates = new();

        if (easyCount > 0)
            candidates.Add(sorted[Random.Range(0, easyCount)]);

        candidates.Add(sorted[Random.Range(0, count)]);
        candidates.Add(sorted[Random.Range(0, count)]);


        return candidates;
    }
    private int GetClearScore(int[,] grid, BlockShape blockShape)
    {
        int width = grid.GetLength(0);
        int heigh = grid.GetLength(1);
        int bestScore = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < heigh; y++)
            {
                bool fits = true;
                List<Vector2Int> posCell = new();
                foreach (var item in blockShape.cells)
                {
                    int nx = item.x + x;
                    int ny = y - item.y;
                    if (nx >= width || ny >= heigh || nx < 0 || ny < 0 || grid[nx, ny] != 0)
                    {
                        fits = false;
                        break;
                    }
                    posCell.Add(new(x, y));
                }
                if (!fits)
                {
                    continue;
                }
                int score = 0;
                foreach (var pos in posCell)
                {
                    int rowFilled = 0;
                    int colFilled = 0;
                    for (int w = 0; w < width; w++) if (grid[w, pos.y] == 1) rowFilled++;
                    for (int h = 0; h < heigh; h++) if (grid[pos.x, h] == 1) colFilled++;
                    score += rowFilled + colFilled;
                }
                bestScore = Mathf.Max(bestScore, score);
            }
        }

        return bestScore;
    }
    private void LoadBlock()
    {
        ISaveLoad saveLoad = new SaveLoad();
        SaveDataShape blockData = saveLoad.LoadData<SaveDataShape>(Flags.BLOCK_DATA_FILE);
        if (blockData.blocks.Count > 0)
        {
            for (int i = 0; i < blockData.blocks.Count; i++)
            {
                if (blockData.blocks[i].indexOfShape != -1)// indexOfShape default = -1
                {
                    GameObject shapeSpawn = DisplayBlock(blockData.blocks[i].indexOfShape, blockData.blocks[i].sprite, blockData.blocks[i].id);
                    if (!blockData.blocks[i].active)
                    {
                        shapeSpawn.SetActive(false);
                    }
                }
            }
        }
        else
        {
            GeneratorBlock();
        }
        CheckEndGame();
    }
    private GameObject DisplayBlock(int indexBlock, string spriteName, int index)
    {
        BlockShape blockShape = allShape[indexBlock];
        Sprite sprite = GetSprite(spriteName);
        GameObject parrentObj = InstantiateParent(defaultBlock, transform);
        parrentObj.GetComponent<MultipleDrag>().blockShape = blockShape;
        foreach (var item in blockShape.cells)
        {
            GameObject generator = Instantiate(cell, parrentObj.transform);
            generator.GetComponent<RectTransform>().anchoredPosition = new Vector2(100 * item.x, 100 * item.y);
            generator.GetComponent<Image>().sprite = sprite;
        }
        float x = blockShape.cells.Max(c => c.x);
        float y = blockShape.cells.Max(c => c.y);

        Vector2 position = new(x * -25, 300 - 300 * index - y * 25);//0,25,50,100
        parrentObj.transform.localPosition = position;

        currentBlocks.Add(new(index, indexBlock, parrentObj));
        return parrentObj;
    }
    private Sprite GetSprite(string spriteName)
    {
        if (spriteName == null)
        {
            return SpriteController.Instance.GetRandomSpriteBlock();
        }
        return SpriteController.Instance.GetSpriteBlock(spriteName);

    }
    private GameObject InstantiateParent(GameObject patternObj, Transform transformParent)
    {
        GameObject patternClone = Instantiate(patternObj, transformParent);
        patternClone.transform.SetParent(transform);
        patternClone.transform.localScale = new(0.5f, 0.5f, 0.5f);
        patternClone.name = currentBlocks.Count.ToString();
        MultipleDrag multipleDrag = patternClone.GetComponent<MultipleDrag>();
        return patternClone;
    }
    public SaveDataShape GetBlockData()
    {
        List<BlockData> blockData = new();
        foreach (var item in currentBlocks)
        {
            blockData.Add(new(item.indexInAllShape, item.gameObject.GetComponentInChildren<Image>().sprite.name, item.index, item.gameObject.activeSelf));
        }
        Debug.Log(blockData.Count);
        return new SaveDataShape() { blocks = blockData };
    }
    private bool CanPlace(int[,] board, BlockShape block)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (CanPlaceAt(board, block, row, col))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CanPlaceAt(int[,] board, BlockShape blockShape, int startRow, int startCol)
    {
        foreach (var item in blockShape.cells)
        {
            int x = item.x + startRow;
            int y = startCol - item.y;
            if (x >= board.GetLength(0) || y >= board.GetLength(1) || x < 0 || y < 0 || board[x, y] != 0)
            {
                return false;
            }
        }

        return true;
    }
    public bool CheckPlace(BlockShape shape)
    {

        if (CanPlace(BoardController.Instance.board, shape))
        {
            return true;
        }
        return false;
    }
    public BlockShape GetShape(int index)
    {
        Debug.Log(index);
        return allShape[index];
    }
    public List<BlockShape> allShape = new()
    {
        new BlockShape(new [] { new Vector2Int(0, 0) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 0) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) }),
        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 2) }),
        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) }),
        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) }),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4) }),

        };
}
