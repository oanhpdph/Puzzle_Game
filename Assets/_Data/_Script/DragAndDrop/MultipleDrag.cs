using Assets._Data._Script.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultipleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // List of objects to drag
    private Canvas canvas;
    private RectTransform m_RectTransform;
    private int row = 0, col = 0;

    private List<GameObject> listChildren;
    private List<GameObject> listDrop;
    private GameObject[,] arrayCell;

    private Image imageBlock;
    private GameObject lastParent;

    private CanvasGroup canvasGroup;
    private bool isMatch = true;
    private Vector3 scale;

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageBlock = GetComponent<Image>();

        scale = m_RectTransform.localScale;
        arrayCell = new GameObject[BoardController.Instance.arrayCell.GetLength(1), BoardController.Instance.arrayCell.GetLength(0)];
        listChildren = new List<GameObject>();
        arrayCell = BoardController.Instance.arrayCell;

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
            listDrop = new List<GameObject>();
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

                listDrop.Add(arrayCell[col, row]);
                if (listChildren.Count > 0)
                {
                    DropChildren();
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

        if (isMatch)
        {
            foreach (GameObject item in listDrop)
            {
                Image imageItem = item.GetComponent<Image>();

                imageItem.sprite = imageBlock.sprite;
                imageItem.color = imageBlock.color;
                imageItem.pixelsPerUnitMultiplier = imageBlock.pixelsPerUnitMultiplier;
                imageItem.raycastPadding = imageBlock.raycastPadding;

                item.GetComponent<Cell>().Status = true;
            }
            BoardController.Instance.CheckFull();
            ShapeController.Instance.RandomShape();
            BoardController.Instance.AddScore(float.Parse(gameObject.name[..1]));
            AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetCollidingClip());
            Destroy(gameObject);
        }
        else
        {
            transform.SetParent(lastParent.transform);

            m_RectTransform.localScale = scale;
            m_RectTransform.anchoredPosition = Vector3.zero;
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

            if (arrayCell[colChildren, rowChildren].GetComponent<Cell>().Status == true)
            {
                isMatch = false;
                return;
            }
            listDrop.Add(arrayCell[colChildren, rowChildren]);
        }
    }
}
