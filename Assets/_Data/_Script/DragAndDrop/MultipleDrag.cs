using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Dictionary<GameObject, int[,]> AllCurrentBlock;

    private RectTransform m_RectTransform;
    private int row = 0, col = 0;

    private List<GameObject> listChildren;
    private List<int[,]> listDrop;

    private GameObject[,] arrayCell;
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
        arrayCell = CellManager._Cells;

        listChildren = new List<GameObject>();
        foreach (Transform item in gameObject.transform)
        {
            listChildren.Add(item.gameObject);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Cursor.lockState = CursorLockMode.Confined;

        lastParent = transform.parent.gameObject;
        lastPosition = transform.localPosition;

        transform.localScale = Vector3.one;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            listDrop = new List<int[,]>();
            GameObject panel = GetPanelUnderMouse(eventData);

            if (panel != null)
            {
                isMatch = true;
                transform.SetParent(panel.transform);


                col = Mathf.RoundToInt(4 + m_RectTransform.anchoredPosition.x / 100);
                row = Mathf.RoundToInt(4 - m_RectTransform.anchoredPosition.y / 100);

                if (col < 0 || row < 0 || col > arrayCell.GetLength(1) - 1 || row > arrayCell.GetLength(0) - 1)
                {
                    isMatch = false;
                    return;
                }
            }
            else
            {
                transform.SetParent(lastParent.transform);
                isMatch = false;
                listDrop.Clear();
            }

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        HandleDrop(eventData);
        Cursor.lockState = CursorLockMode.None;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    private void HandleDrop(PointerEventData eventData)
    {
        DropChildren();
        if (isMatch)
        {
            foreach (int[,] item in listDrop)
            {
                CellManager.cells[item[0, 0], item[0, 1]] = 1;
                GameObject block = arrayCell[item[0, 0], item[0, 1]];
                Image imageItem = block.GetComponent<Image>();

                imageItem.sprite = transform.GetChild(0).GetComponent<Image>().sprite;
                imageItem.color = new Color(1, 1, 1, 1);
                imageItem.pixelsPerUnitMultiplier = 100;

                block.GetComponent<Cell>().Status = 1;
            }

            AllCurrentBlock.Remove(gameObject);
            Destroy(gameObject);
            BoardController.Instance.CheckFull();
            BoardController.Instance.UI_Score.AddScore(transform.childCount, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetCollidingClip());
        }
        else
        {
            transform.SetParent(lastParent.transform);

            m_RectTransform.localScale = scale;
            m_RectTransform.anchoredPosition = lastPosition;
        }

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
    private void DropChildren()
    {
        foreach (GameObject item in listChildren)
        {
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            int colChildren = col;
            int rowChildren = row;

            colChildren += (int)rectTransform.anchoredPosition.x / 100;

            rowChildren -= (int)rectTransform.anchoredPosition.y / 100;

            if (colChildren < 0 || rowChildren < 0 || colChildren > arrayCell.GetLength(1) - 1 || rowChildren > arrayCell.GetLength(0) - 1)
            {
                isMatch = false;
                return;
            }

            if (arrayCell[rowChildren, colChildren].GetComponent<Cell>().Status == 1)
            {
                isMatch = false;
                return;
            }
            listDrop.Add(new int[,] { { rowChildren, colChildren } });

        }

    }
}
