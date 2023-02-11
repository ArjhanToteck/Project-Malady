using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    float offset = 65f;

    void Update()
    {
        float prefferedWidth = GetComponent<Text>().preferredWidth;
        GetComponent<RectTransform>().sizeDelta = new Vector2(prefferedWidth + offset, GetComponent<RectTransform>().sizeDelta.y);
    }
}
