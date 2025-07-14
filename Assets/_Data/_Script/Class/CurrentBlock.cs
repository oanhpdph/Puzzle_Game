using UnityEngine;

public class CurrentBlock
{
    public int index;
    public int indexInAllShape;
    public GameObject gameObject;
    public CurrentBlock(int index, int indexInAllShape, GameObject gameObject)
    {
        this.index = index;
        this.indexInAllShape = indexInAllShape;
        this.gameObject = gameObject;
    }
}
