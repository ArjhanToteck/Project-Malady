using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeySelect : MonoBehaviour
{
    InputField input;
    object keyCode;
    FieldInfo fieldInfo;

    void Start()
    {
        // gets field info of current setting
        fieldInfo = typeof(Controls).GetField(transform.parent.name.ToLower());

        // gets input
        input = GetComponent<InputField>();

        // sets input text to selected control
        input.text = fieldInfo.GetValue(GameSettings.Controls).ToString();

        //Adds a listener to the main input field and invokes a method when the value changes.
        input.onValueChanged.AddListener(delegate { OnChange(); });
    }

    void OnGUI()
    {

        // checks if key is pressed and input box focused
        if (Event.current.isKey && Event.current.type == EventType.KeyDown && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            // checks if no keycode
            if (Event.current.keyCode.ToString() == "None")
            {
                keyCode = null;
            } 
            else
            {
                keyCode = Event.current.keyCode;
            }

            // sets input text to blank so inputed string can be used instead in OnChange
            input.text = "";
        }
    }

    void OnChange()
    {
        if (keyCode != null)
        {
            // sets input to keycode
            if(input.text != keyCode.ToString()) input.text = keyCode.ToString();

            // sets control in game settings to keyCode
            fieldInfo.SetValue(GameSettings.Controls, keyCode);
        }
        else
        {
            if (input.text.Length > 1)
            {
                // limits text to 1 character
                input.text = input.text.Substring(input.text.Length - 1);
            }
            else
            {
                // sets control in game settings to input.text (since OnGUI is called before text is updated)
                fieldInfo.SetValue(GameSettings.Controls, input.text);
            }
        }
    }
}
