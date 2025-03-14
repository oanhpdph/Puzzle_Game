using Assets._Data._Script.Class;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Data._Script.Controller
{
    public class SaveController : MonoBehaviour
    {
        private static SaveController instance { get; set; }
        public static SaveController Instance => instance;

        [SerializeField] private GameObject boardObject;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private TextMeshProUGUI goalScore;
        private SaveData[] arrSaveData;
        private string fileName = "SavePuzzleGame.json";
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public void SaveGame()
        {
            SaveShape();
            SaveCell();
            SaveScore();
        }
        private void SaveShape()
        {
            int i = 1;
            foreach (GameObject item in ShapeController.Instance.listPosition)
            {
                PlayerPrefs.SetString("Shape" + i, item.transform.GetChild(0).name);
                i++;
            }
        }
        private void SaveCell()
        {
            AddCell();
            SaveList save = new() { arrData = arrSaveData };
            Debug.Log(Application.persistentDataPath + "/" + fileName);
            string json = JsonUtility.ToJson(save);
            string path = Application.persistentDataPath + "/" + fileName;
            System.IO.File.WriteAllText(path, json);
        }
        public void AddCell()
        {
            arrSaveData = new SaveData[boardObject.transform.childCount];
            int i = 0;
            foreach (Transform cell in boardObject.transform)
            {
                SaveData data = new();
                data.spriteName = cell.GetComponent<Image>().sprite.name;
                data.status = cell.GetComponent<Cell>().Status;
                arrSaveData[i] = data;
                i++;
            }
        }
        public SaveData[] LoadCell()
        {
            string path = Application.persistentDataPath + "/" + fileName;
            SaveList save = new();
            if (File.Exists(path))
            {
                string data = System.IO.File.ReadAllText(path);
                save = JsonUtility.FromJson<SaveList>(data);
            }
            else
            {
                Debug.Log("file not found");
            }
            return save?.arrData;

        }

        public void ClearFile()
        {
            string path = Application.persistentDataPath + "/" + fileName;
            System.IO.File.WriteAllText(path, "");
            PlayerPrefs.SetFloat("currentScore", 0);
        }
        public void SaveScore()
        {
            float currentScore = PlayerPrefs.GetFloat("currentScore");
            if (currentScore > PlayerPrefs.GetFloat("goalScore"))
            {
                PlayerPrefs.SetFloat("goalScore", currentScore);
            }
            score.text = currentScore.ToString();
            goalScore.text = PlayerPrefs.GetFloat("goalScore").ToString();
        }
    }

    [System.Serializable]
    public class SaveList
    {
        public SaveData[] arrData;
    }
}