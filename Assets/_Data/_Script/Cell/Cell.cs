using UnityEngine;


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
}
