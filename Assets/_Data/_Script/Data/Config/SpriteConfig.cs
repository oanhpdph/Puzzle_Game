using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Sprite Assets", fileName = "Sprite Assets")]
public class SpriteConfig : ScriptableObject
{
    public List<Sprite> sprite;
    public Sprite spriteDefault;
    public Sprite GetRandomSpriteBlock()
    {
        int random = Random.Range(0, sprite.Count - 1);
        return sprite[random];
    }
    public Sprite GetSpriteBlock(string name)
    {
        if (name == null) return null;
        foreach (Sprite sprite in sprite)
        {
            if (name.Equals(sprite.name))
            {
                return sprite;
            }
        }
        return null;
    }
    public Sprite GetSpriteDefault() { return spriteDefault; }
}