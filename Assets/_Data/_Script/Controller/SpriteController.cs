using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{

    [SerializeField] private List<Sprite> listSpriteDimond;
    [SerializeField] private Sprite spriteDefault;

    private static SpriteController instance { get; set; }
    public static SpriteController Instance => instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Sprite GetRandomSpriteBlock()
    {
        int random = Random.Range(0, listSpriteDimond.Count - 1);
        return listSpriteDimond[random];
    }
    public Sprite GetSpriteBlock(string name)
    {
        foreach (Sprite sprite in listSpriteDimond)
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
