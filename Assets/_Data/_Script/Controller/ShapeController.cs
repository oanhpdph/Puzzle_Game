using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Data._Script.Controller
{
    public class ShapeController : MonoBehaviour
    {
        private static ShapeController instance { get; set; }
        public static ShapeController Instance => instance;

        [SerializeField] private GameObject shapeReserve;
        [SerializeField] private GameObject shapeManager;
        [SerializeField] private GameObject panelEndGame;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private TextMeshProUGUI goalScore;

        [HideInInspector] public bool endGame = true;
        private List<GameObject> listShape;

        [HideInInspector]
        public List<GameObject> listPosition;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            AddChildren();
            AddShape();
            panelEndGame.SetActive(false);
        }
        private void Start()
        {
            LoadShape();
            RandomShape();
        }
        private void AddChildren()
        {
            listPosition = new List<GameObject>();
            foreach (Transform transform in shapeReserve.transform)
            {

                listPosition.Add(transform.gameObject);
            }
        }

        private void AddShape()
        {
            listShape = new List<GameObject>();
            foreach (Transform transform in shapeManager.transform)
            {

                listShape.Add(transform.gameObject);
            }
        }

        public void RandomShape()
        {
            foreach (GameObject item in listPosition)
            {
                if (item.transform.childCount == 0)
                {
                    int index = RandomIndex();
                    GameObject result = Instantiate(listShape[index], item.transform);
                    result.name = listShape[index].name;
                    RenderShape(result);
                }
            }
            EndGame();
        }
        public void RenderShape(GameObject result)
        {

            RectTransform rectTransform = result.GetComponent<RectTransform>();

            rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            Sprite sprite = SpriteController.Instance.GetRandomSpriteBlock();
            result.GetComponent<Image>().sprite = sprite;
            foreach (Transform transform in result.transform)
            {
                transform.GetComponent<Image>().sprite = sprite;
            }
        }

        private int RandomIndex()
        {
            return Random.Range(0, listShape.Count - 1);
        }

        private void EndGame()
        {
            foreach (GameObject item in listPosition)
            {
                if (!BoardController.Instance.CheckEndGame(item.transform.GetChild(0).gameObject))
                {
                    endGame = false;
                    break;
                }
            }

            if (endGame)
            {
                SaveController.Instance.SaveScore();
                SaveController.Instance.ClearFile();
                PlayerPrefs.SetInt("isHigh", 0);
                panelEndGame.SetActive(true);
                AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetGameOverClip());

            }
        }

        private void LoadShape()
        {
            int i = 1;
            foreach (GameObject item in listPosition)
            {
                string name = PlayerPrefs.GetString("Shape" + i);
                if (name == null)
                {
                    break;
                }
                foreach (GameObject prefab in listShape)
                {
                    if (name.Equals(prefab.name))
                    {
                        GameObject shape = Instantiate(prefab, item.transform);
                        shape.name = name;
                        RenderShape(shape);
                    }
                }
                i++;
            }
        }
    }
}