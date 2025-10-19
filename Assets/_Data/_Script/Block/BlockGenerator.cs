using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class BlockGenerator : Singleton<BlockGenerator>
{
    [SerializeField] private List<GameObject> ListBlocks;
    private List<CurrentBlock> currentBlocks = new();
    private List<Vector2Int> direction = new();

    private async Task<GameObject> LoadAssets(string name)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(name);

        await handle.Task; // Chờ đến khi handle hoàn thành

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load Addressable: {name}");
            return null;
        }
    }
    private void Start()
    {
        direction.Add(Vector2Int.left);
        direction.Add(Vector2Int.right);
        direction.Add(Vector2Int.up);
        direction.Add(Vector2Int.down);
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
                if (CanPlace(BoardController.Instance.board, GetShape(item.indexInAllShape)))
                {
                    return;
                }
            }
        }
        HUDSystem.Instance.Show<EndPanel>();
    }
    public void GeneratorBlock()
    {
        List<int> priorityShapes = GetSmartPriorityShape();

        for (int i = 0; i < priorityShapes.Count; i++)
        {
            DisplayBlock(priorityShapes[i], null, currentBlocks.Count);
        }
        GameController.Instance.SaveDataShape = GetBlockData();
    }

    private List<int> GetSmartPriorityShape()
    {

        List<int> candidates;
        if (GameController.Instance.ScoreData.currentScore == 0)
        {
            candidates = new List<int>() {
                Random.Range(0, ListBlocks.Count),
                Random.Range(0, ListBlocks.Count),
                Random.Range(0, ListBlocks.Count), };
            return candidates;
        }
        List<(int index, int score, int adjacentCell)> scored = new();
        List<(int index, BlockShape shape)> priorityShape = GetPriorityShape();
        for (int i = 0; i < priorityShape.Count; i++)
        {
            int bestScore, bestAdjacentCell;
            (bestScore, bestAdjacentCell) = GetConditionPoint(BoardController.Instance.board, priorityShape[i].shape);
            scored.Add((priorityShape[i].index, bestScore, bestAdjacentCell));
        }
        if (scored.Count == 0) return null;

        int maxScore = scored.Max(s => s.score);
        var sorted = scored.OrderByDescending(s => (s.adjacentCell == ListBlocks[s.index].GetComponent<MultipleDrag>().blockShape.maxAdjacentCell))
            .ThenByDescending(s => s.score)
            .ThenByDescending(s => s.adjacentCell).Select(s => s.index).ToList();

        int count = sorted.Count;

        int easyCount = 1;
        int middleCount = 6;
        int hardCount = count;
        if (GameController.Instance.ScoreData.isHighScore)
        {
            easyCount *= 2;
            middleCount *= 2;
        }
        candidates = new()
        {
            sorted[Random.Range(0, Mathf.Clamp(easyCount,0,count))],
            sorted[Random.Range(0, Mathf.Clamp(middleCount,0,count))],
            sorted[Random.Range(0, count)]
        };
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
            Vector2Int nearCell = origin + item;
            if (nearCell.x >= grid.GetLength(0) || nearCell.y >= grid.GetLength(1) || nearCell.x < 0 || nearCell.y < 0)
            {
                counter++;
                continue;
            }
            if (grid[nearCell.x, nearCell.y] == 1)
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
    private List<(int, BlockShape)> GetPriorityShape()
    {
        List<(int, BlockShape)> priorityShape = new();
        for (int i = 0; i < ListBlocks.Count; i++)
        {
            if (CanPlace(BoardController.Instance.board, ListBlocks[i].GetComponent<MultipleDrag>().blockShape))
                priorityShape.Add((i, ListBlocks[i].GetComponent<MultipleDrag>().blockShape));
        }
        return priorityShape;
    }
    public void LoadBlock()
    {
        SaveDataShape blockData = GameController.Instance.SaveDataShape;

        if (currentBlocks.Count > 0)
        {
            foreach (var item in currentBlocks)
            {
                Destroy(item.gameObject);
            }
            currentBlocks.Clear();
        }
        if (blockData != null && blockData.blocks.Count > 0)
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
        BlockShape blockShape = ListBlocks[indexBlock].GetComponent<MultipleDrag>().blockShape;
        ListBlocks[indexBlock].CreatePool();
        ListBlocks[indexBlock].Use(out var cloneObj);
        cloneObj.transform.SetParent(transform);
        cloneObj.GetComponent<MultipleDrag>().blockShape = blockShape;
        cloneObj.GetComponent<RenderBlock>().RenderSprite(spriteName);
        cloneObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        float x = blockShape.cells.Max(c => c.x);
        float y = blockShape.cells.Max(c => c.y);

        Vector2 position = new(x * -25, 300 - 300 * index - y * 25);//0,25,50,100
        cloneObj.transform.localPosition = position;

        currentBlocks.Add(new(index, indexBlock, cloneObj));
        return cloneObj;
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
        return ListBlocks[index].GetComponent<MultipleDrag>().blockShape;
    }
    public List<BlockShape> allShape = new()
    {
        new BlockShape(new [] { new Vector2Int(0, 0) },4),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0) },6),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1) },6),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },8),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },8),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1) },8),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1) },8),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },8),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 0) },8),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },8),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },8),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },8),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },10),
        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },10),

        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) },10),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2) },10),

        new BlockShape(new [] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(1, 0) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },10),


        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 2) },9),
        new BlockShape(new [] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },9),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 2) },9),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },9),

        new BlockShape(new [] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 0) },9),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },9),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 1) },9),
        new BlockShape(new [] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(2, 0) },9),


        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },10),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 2) },10),

        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },12),

        new BlockShape(new []{ new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(2, 1)},12),

        new BlockShape(new [] { new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 2) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },12),

        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) },12),
        new BlockShape(new [] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4) },12),

        };
}
