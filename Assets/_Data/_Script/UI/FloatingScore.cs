using TMPro;
using UnityEngine;

public class FloatingScore : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public float lifeTime = 1.5f;
    public float floatUpSpeed = 20f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;
    }

    public void SetScore(float score)
    {
        textMeshProUGUI.text = "+" + score;
    }
}
