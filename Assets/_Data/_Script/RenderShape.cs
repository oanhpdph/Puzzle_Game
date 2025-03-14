using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RenderShape : MonoBehaviour
{
    [SerializeField] private List<GameObject> listShape;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string nameBlock;
    [SerializeField] private GameObject manager;
    private string firstName = " ";

    private void Start()
    {
        Render();

    }

    private void Render()
    {
        int i = 1;
        string pathManager = AssetDatabase.GetAssetPath(manager);

        foreach (GameObject shape in listShape)
        {
            GameObject parent = new();
            if (!shape.name.StartsWith(firstName))
            {
                firstName = shape.name.ToString()[..2];
                i = 1;
            }
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(pathManager[..(pathManager.LastIndexOf("/") + 1)] + firstName + nameBlock + " " + i + ".prefab");
            parent = Instantiate(shape, transform);

            Image image = parent.GetComponent<Image>();
            image.sprite = sprite;

            foreach (Transform item in shape.transform)
            {
                item.name = nameBlock;
                item.GetComponent<Image>().sprite = sprite;

            }
            i++;
            PrefabUtility.SaveAsPrefabAsset(parent, uniquePath);

        }
    }
}
