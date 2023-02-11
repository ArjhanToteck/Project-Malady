using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsTabSwitch : MonoBehaviour
{
    GameObject general;
    GameObject players;
    GameObject diseases;
    GameObject[] menus;

    GameObject eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        general = transform.Find("General").gameObject;
        players = transform.Find("Players").gameObject;
        diseases = transform.Find("Diseases").gameObject;

        menus = new GameObject[] { general, players, diseases };

        eventSystem = GameObject.Find("EventSystem");

        general.transform.Find("FileFolderTab").GetComponent<Button>().onClick.AddListener(delegate { OpenMenu(general); });
        players.transform.Find("FileFolderTab").GetComponent<Button>().onClick.AddListener(delegate { OpenMenu(players); });
        diseases.transform.Find("FileFolderTab").GetComponent<Button>().onClick.AddListener(delegate { OpenMenu(diseases); });
    }

    void OpenMenu(GameObject menu)
    {
        // unselects any selected button
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

        int i = 0;

        // loops through each menu
        foreach (GameObject currentMenu in menus)
        {

            // skips over clicked menu
            if (currentMenu == menu)
            {
                // makes focused menu ligher and therefore appear on top
                menu.transform.Find("FileFolderTab").GetComponent<Image>().color = Color.white;
            }
            else
            {
                i++;

                // darkens color depending on alignment from left to right
                currentMenu.transform.Find("FileFolderTab").GetComponent<Image>().color = new Color(1 - (i * 0.1f), 1 - (i * 0.1f), 1 - (i * 0.1f));

                // makes innactive
                currentMenu.transform.Find("Content").gameObject.SetActive(false);
            }            
        }

        // sets menu to be active
        menu.transform.Find("Content").gameObject.SetActive(true);
    }
}
