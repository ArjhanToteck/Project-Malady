using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler
{
    public Text textElement;
    public string originalText;
    public bool selected = false;

    void Start()
    {
        // collects info about text
        textElement = transform.Find("Text").GetComponent<Text>();
        originalText = textElement.text;

        // first button
        if(transform.GetSiblingIndex() == 0)
        {
            textElement.text = ">" + originalText;
            selected = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // unselects all
        for(int i = 0; i < transform.parent.childCount; i++)
        {
            ButtonEffects currentButton = transform.parent.GetChild(i).GetComponent<ButtonEffects>();
            currentButton.selected = false;
            currentButton.textElement.text = currentButton.originalText;
        }

        // selects current button
        selected = true;
        textElement.text = ">" + originalText;
    }

    void Update()
    {
        if (selected)
        {
            // checks for down arrow
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // selects item below
                selected = false;
                textElement.text = originalText;
                ButtonEffects nextButton;

                // gets next button
                if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                {
                    nextButton = transform.parent.GetChild(0).GetComponent<ButtonEffects>();
                }
                else
                {
                    nextButton = transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponent<ButtonEffects>();
                }

                StartCoroutine(SelectNextButton(nextButton));
            }

            // checks for up arrow
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // selects item above
                selected = false;
                textElement.text = originalText;
                ButtonEffects nextButton;

                // gets previous button
                if (transform.GetSiblingIndex() == 0)
                {
                    nextButton = transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<ButtonEffects>();
                }
                else
                {
                    nextButton = transform.parent.GetChild(transform.GetSiblingIndex() - 1).GetComponent<ButtonEffects>();
                }

                StartCoroutine(SelectNextButton(nextButton));
            }

            // checks for enter or space
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    IEnumerator SelectNextButton(ButtonEffects nextButton)
    {
        // wait one frame
        yield return 0;

        // selects next button
        nextButton.selected = true;
        nextButton.textElement.text = ">" + nextButton.originalText;
    }
}
