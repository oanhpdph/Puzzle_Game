using UnityEngine;
using UnityEngine.UI;


public class Cell : MonoBehaviour
{
    public int Status = 0;
    public int Row;
    public int Col;

    public void SetPosition(int row, int cow)
    {
        this.Row = row;
        this.Col = cow;
    }
    public (int row, int col) GetPosition() => (Row, Col);
    public void ResetCell()
    {
        Image imageCell = GetComponent<Image>();

        imageCell.sprite = SpriteController.Instance.GetSpriteDefault();
        imageCell.color = new(0.9f, 0.65f, 0.4f, 1);
        imageCell.pixelsPerUnitMultiplier = 1;

        Status = 0;
    }
}
