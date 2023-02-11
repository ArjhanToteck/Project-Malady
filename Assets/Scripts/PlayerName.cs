using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    public string playerName;
    GameObject playerNameObject;

    // Start is called before the first frame update
    void Start()
    {
        playerNameObject = Instantiate(FindObjectOfType<GameManager>().playerNameTemplate);
        playerNameObject.GetComponent<Text>().text = playerName;
        playerNameObject.name = playerName;
        playerNameObject.transform.SetParent(GameObject.Find("WorldspaceCanvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        playerNameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);
    }

    public void ChangeColor(Color color)
    {
        playerNameObject.GetComponent<Text>().color = color;
    }
}
