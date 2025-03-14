using System.Collections.Generic;
using UnityEngine;

namespace Assets._Data._Script.Controller
{
    public class SpriteController : MonoBehaviour
    {

        [SerializeField] private List<Sprite> listSpriteDimond;

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

    }
}