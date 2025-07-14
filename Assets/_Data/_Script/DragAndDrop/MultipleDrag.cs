using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform m_RectTransform;
    private int row = 0, col = 0;
    public BlockShape blockShape;
    private List<GameObject> pieceOfShape = new();
    private List<GameObject> suitableLocation = new();

    private GameObject[,] grid;
    private GameObject lastParent;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool isMatch = true;
    private Vector3 scale;
    private Vector2 lastPosition;

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        scale = m_RectTransform.localScale;
        grid = BoardController.Instance.cells;


        lastParent = transform.parent.gameObject;
        lastPosition = transform.localPosition;
        AddPieceOfShape();
    }

    private void AddPieceOfShape()
    {
        foreach (Transform item in gameObject.transform)
        {
            pieceOfShape.Add(item.gameObject);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Cursor.lockState = CursorLockMode.Confined;
            transform.localScale = Vector3.one;
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
            m_RectTransform.anchorMax = new Vector2(0, 1);
            m_RectTransform.anchorMin = new Vector2(0, 1);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            transform.position = mousePos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        ResetUIMatch();
        GameObject panel = GetPanelUnderMouse(eventData);

        if (panel != null)
        {
            isMatch = true;
            transform.SetParent(panel.transform);

            row = Mathf.FloorToInt(m_RectTransform.anchoredPosition.x / 100);
            col = Mathf.FloorToInt(-m_RectTransform.anchoredPosition.y / 100);
            if (IsOutOfIndex(row, col))
            {
                isMatch = false;
                return;
            }
            DropChildren();
        }
        else
        {
            transform.SetParent(lastParent.transform);
            isMatch = false;
            suitableLocation.Clear();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        HandleDrop(eventData);
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandleDrop(PointerEventData eventData)
    {
        if (isMatch)
        {
            foreach (var item in suitableLocation)
            {
                Cell cell = item.GetComponent<Cell>();
                (int row, int col) = cell.GetPosition();

                BoardController.Instance.board[row, col] = 1;
                Image imageItem = item.GetComponent<Image>();
                imageItem.sprite = transform.GetChild(0).GetComponent<Image>().sprite;
                imageItem.color = new Color(1, 1, 1, 1);
                imageItem.pixelsPerUnitMultiplier = 100;

                cell.Status = 1;
                BoardController.Instance.CheckCol(col);
                BoardController.Instance.CheckRow(row);
            }
            BoardController.Instance.CheckFull();

            BoardController.Instance.UI_Score.AddScore(transform.childCount, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetCollidingClip());
            gameObject.SetActive(false);
            GameController.Instance.DropShapeAction();

        }

        BackToPosition();
    }

    private void DropChildren()
    {
        suitableLocation = new();
        foreach (var item in blockShape.cells)
        {
            int x = row + item.x;
            int y = col - item.y;

            if (IsOutOfIndex(x, y) || grid[x, y].GetComponent<Cell>().Status == 1)
            {
                isMatch = false;
                return;
            }
            suitableLocation.Add(grid[x, y]);
        }
        LoadUIMatch();
    }
    private GameObject GetPanelUnderMouse(PointerEventData eventData)
    {
        // Perform a raycast to detect UI elements under the mouse
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        // Find the first panel in the results
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Board")) // Use a tag or any condition to identify panels
            {
                return result.gameObject;
            }
        }

        return null;
    }

    private void LoadUIMatch()
    {
        if (suitableLocation.Count == 0) return;
        foreach (var item in suitableLocation)
        {
            Image imageItem = item.GetComponent<Image>();
            imageItem.sprite = transform.GetChild(0).GetComponent<Image>().sprite;
            imageItem.color = new Color(1, 1, 1, 1);
            imageItem.pixelsPerUnitMultiplier = 100;
            //imageItem.color = new Color(0.7f, 0.8f, 0.4f, 1);
        }
    }
    private void ResetUIMatch()
    {
        if (suitableLocation.Count == 0) return;
        foreach (var item in suitableLocation)
        {
            Image imageItem = item.GetComponent<Image>();
            imageItem.sprite = SpriteController.Instance.GetSpriteDefault();
            imageItem.color = new Color(0.9f, 0.65f, 0.4f, 1);
            imageItem.pixelsPerUnitMultiplier = 1;

        }
    }
    private bool IsOutOfIndex(int row, int col)
    {
        if (col < 0 || row < 0 || col >= grid.GetLength(1) || row >= grid.GetLength(0))
        {
            isMatch = false;
            return true;
        }
        return false;
    }
    public void BackToPosition()
    {
        m_RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        m_RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        transform.SetParent(lastParent.transform);
        m_RectTransform.localScale = scale;
        m_RectTransform.anchoredPosition = lastPosition;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
}
