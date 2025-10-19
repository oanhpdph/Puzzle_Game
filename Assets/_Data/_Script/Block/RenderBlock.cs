using UnityEngine;
using UnityEngine.UI;

public class RenderBlock : MonoBehaviour
{
    public void RenderSprite(string spriteName)
    {
        Sprite sprite = GameController.Instance.SpriteConfig.GetSpriteBlock(spriteName);
        if (sprite == null)
            sprite = GameController.Instance.SpriteConfig.GetRandomSpriteBlock();
        foreach (Transform item in transform)
        {
            item.GetComponent<Image>().sprite = sprite;
        }
    }
}
