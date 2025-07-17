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
    private List<Vector2> direction = new();
    private void Start()
    {
        direction.Add(Vector2.left);
        direction.Add(Vector2.right);
        direction.Add(Vector2.up);
        direction.Add(Vector2.down);

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
        List<(int index, int score, int adjacentCell)> scored = new();
        for (int i = 0; i < allShape.Count; i++)
        {
            int bestScore, bestAdjacentCell;
            (bestScore, bestAdjacentCell) = GetConditionPoint(BoardController.Instance.board, allShape[i]);
            scored.Add((i, bestScore, bestAdjacentCell));
        }
        if (scored.Count == 0) return null;

        int maxScore = scored.Max(s => s.score);
        var sorted = scored.OrderByDescending(s => s.adjacentCell).ThenByDescending(s => s.score).Select(s => s.index).ToList();

        int count = sorted.Count;
        float propotion = Mathf.Clamp01(0.3f + GameController.Instance.scoreData.currentScore / 500);
        int easyCount = Mathf.CeilToInt(count * propotion);

        List<int> candidates = new();

        if (easyCount > 0)
        {
            candidates.Add(sorted[Random.Range(0, easyCount)]);
            candidates.Add(sorted[Random.Range(0, easyCount)]);
            candidates.Add(sorted[Random.Range(0, easyCount)]);
        }
        return candidates;
    }
    private (int, int) GetConditionPoint(int[,] grid, BlockShape blockShape)
    {
        int width = grid.GetLength(0);
        int heigh = grid.GetLength(1);
        int bestScore = 0;
        int bestAdjacentCell = 0;
        int[,] copyBoard = CopyBoard(grid);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < heigh; y++)// loop cell in board
            {
                bool fits = true;
                List<Vector2Int> posCell = new();
                foreach (var item in blockShape.cells)// loop each cell in block shape
                {
                    int nx = item.x + x;
                    int ny = y - item.y;
                    if (nx >= width || ny >= heigh || nx < 0 || ny < 0 || grid[nx, ny] != 0)
                    {
                        fits = false;
                        break;
                    }
                    copyBoard[nx, ny] = 1;
                    posCell.Add(new(nx, ny));
                }
                if (!fits)
                {
                    ResetBoard(copyBoard, posCell);
                    continue;
                }

                int score = 0;
                int adjacentCell = 0;

                foreach (var pos in posCell)
                {
                    score += GetClearScore(pos, copyBoard);
                    adjacentCell += GetNumberAdjacentCell(pos, grid);
                }
                bestAdjacentCell = Mathf.Max(bestAdjacentCell, adjacentCell);
                bestScore = Mathf.Max(bestScore, score);
                ResetBoard(copyBoard, posCell);
            }
        }

        return (bestScore, bestAdjacentCell);
    }

    private void ResetBoard(int[,] board, List<Vector2Int> posCell)
    {
        foreach (Vector2Int item in posCell)
        {
            board[item.x, item.y] = 0;
        }
    }
    private int GetNumberAdjacentCell(Vector2Int origin, int[,] grid)
    {
        int counter = 0;
        foreach (var item in direction)
        {
            Vector2 nearCell = origin + item;
            if (grid[origin.x, origin.y] == 1)
            {
                counter++;
            }
        }
        return counter;
    }
    private int GetClearScore(Vector2Int origin, int[,] grid)
    {
        int width = grid.GetLength(0);
        int heigh = grid.GetLength(1);

        int score = 0;
        int rowFilled = 0;
        int colFilled = 0;
        for (int w = 0; w < width; w++) if (grid[w, origin.y] == 1) rowFilled++;
        for (int h = 0; h < heigh; h++) if (grid[origin.x, h] == 1) colFilled++;
        return score += rowFilled / 9 + colFilled / 9;
    }
    private int[,] CopyBoard(int[,] grid)
    {
        int[,] boardCopy = new int[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < boardCopy.GetLength(0); i++)
        {
            for (int j = 0; j < boardCopy.GetLength(1); j++)
            {
                boardCopy[i, j] = grid[i, j];
            }
        }
        return boardCopy;
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
