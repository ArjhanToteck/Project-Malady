using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    public GameObject eventSystem;

    public GameObject humanPlayerTemplate;
    public GameObject botPlayerTemplate;

    void Start()
    {
        // converts all existing players to GameObjects
        for (int i = 0; i < GameSettings.Players.Length; i++)
        {
            PlayerToObject(i);
            
            // resizes scrollview if neccessary
            if (i > 1)
            {
                transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 166.5f);
            }
        }
    }
    GameObject PlayerToObject(int playerIndex)
    {
        GameObject playerObject;
        Player player = GameSettings.Players[playerIndex];

        // if bot
        if (player.bot)
        {
            playerObject = Instantiate(botPlayerTemplate);

            // duplicate
            playerObject.transform.Find("Duplicate").GetComponent<Button>().onClick.AddListener(delegate {
                // deselects button
                eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

                // copies player data over to duplicate
                Stream stream = new MemoryStream();
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, player);
                stream.Seek(0, SeekOrigin.Begin);
                Player clonedPlayer = (Player)formatter.Deserialize(stream);
                clonedPlayer.name = "CPU" + (transform.childCount - 2);

                // converts GameSettings.Players to list to easily add clonedPlayer
                List<Player> newPlayers = new List<Player>(GameSettings.Players);
                newPlayers.Add(clonedPlayer);

                // adds duplicated player to gamesettings.players
                GameSettings.Players = newPlayers.ToArray();

                // sets player once again to avoid problems
                player = GameSettings.Players[playerObject.transform.GetSiblingIndex() - 2];

                // creates object from duplicated player and sets delete to active
                GameObject playerDuplicate = PlayerToObject(transform.childCount - 2);
                playerDuplicate.transform.Find("Delete").gameObject.SetActive(true);

                playerObject.transform.Find("Delete").gameObject.SetActive(true);

                // resizes scrollview
                transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 166.5f);
            });

            // delete
            playerObject.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(delegate {
                // deselects button
                eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                
                // resizes gamesettings.players to remove player
                Player[] newPlayers = GameSettings.Players;
                Array.Resize(ref newPlayers, transform.childCount - 3);
                GameSettings.Players = newPlayers;

                // repositions siblings that are below
                for (int i = 0; i < transform.childCount - playerObject.transform.GetSiblingIndex(); i++)
                {
                    // gets sibling
                    RectTransform sibling = transform.GetChild(i + playerObject.transform.GetSiblingIndex()).GetComponent<RectTransform>();

                    // repositions sibling
                    sibling.position += new Vector3(0, 0.3649f * Screen.height, 0);
                }

                // removes delete button if neccessary
                if (transform.childCount == 5)
                {
                    Debug.Log(transform.GetChild(4).Find("Delete").gameObject.name);
                    transform.GetChild(4).Find("Delete").gameObject.SetActive(false);
                    transform.GetChild(3).Find("Delete").gameObject.SetActive(false);
                }

                // destroys gameobject
                Destroy(playerObject);

                // resizes scrollview
                transform.parent.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 166.5f);
            });
        }
        else
        {
            playerObject = Instantiate(humanPlayerTemplate);

            // spectator
            Toggle spectator = playerObject.transform.Find("Spectator").Find("Toggle").GetComponent<Toggle>();
            spectator.isOn = player.spectator;
            spectator.onValueChanged.AddListener(delegate {
                // sets spectator
                player.spectator = spectator.isOn;

                // hides disease if spectator
                playerObject.transform.Find("Disease").gameObject.SetActive(!spectator.isOn);
            });
        }

        // disease
        Dropdown disease = playerObject.transform.Find("Disease").Find("Dropdown").GetComponent<Dropdown>();

        // sets disease if needed

        // checks if assigned specifically
        if (player.assignedSpecifically)
        {
            // this means player was assigned healthy
            if(player.disease == null)
            {
                disease.value = 1; // 1 is healthy
            }
            else // this means player is assigned a specific disease
            {
                // gets options of dropdown
                // clears options
                disease.options = new List<Dropdown.OptionData>();

                // adds random and healthy options
                disease.options.Add(new Dropdown.OptionData("Random"));
                disease.options.Add(new Dropdown.OptionData("Healthy"));

                // adds each disease as an option
                foreach (Disease currentDisease in GameSettings.Diseases)
                {
                    disease.options.Add(new Dropdown.OptionData(currentDisease.name));
                }

                disease.RefreshShownValue();

                disease.value = player.disease.index + 2; // + 2 because of healthy and random
                disease.RefreshShownValue();
            }
        }
        else // this means player disease is supposed to be random
        {
            disease.value = 0; // 0 is random
        }

        // refreshes dropdown
        disease.RefreshShownValue();

        // detect click
        EventTrigger trigger = disease.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;

        entry.callback.AddListener((data) => {
            // clears options
            disease.options = new List<Dropdown.OptionData>();

            // adds random and healthy options
            disease.options.Add(new Dropdown.OptionData("Random"));
            disease.options.Add(new Dropdown.OptionData("Healthy"));

            // adds each disease as an option
            foreach (Disease currentDisease in GameSettings.Diseases)
            {
                disease.options.Add(new Dropdown.OptionData(currentDisease.name));
            }

            disease.RefreshShownValue();
        });

        // adds trigger to dropdown
        trigger.triggers.Add(entry);

        disease.onValueChanged.AddListener(delegate {

            // checks if random
            if (disease.options[disease.value].text == "Random")
            {
                // random, so not assigned specifically
                player.assignedSpecifically = false;
                player.disease = null;
            }
            else
            {
                // not random so assigned specifically
                player.assignedSpecifically = true;

                // checks if not healthy
                if (disease.options[disease.value].text != "Healthy")
                {
                    // makes sure disease has index
                    GameSettings.Diseases[disease.value - 2].index = disease.value - 2;

                    // not healthy so assigns disease
                    player.disease = GameSettings.Diseases[disease.value - 2];
                }
                else
                {
                    player.disease = null;
                }
            }

            disease.RefreshShownValue();
        });

        // player name
        InputField name = playerObject.transform.Find("Name").Find("InputField").GetComponent<InputField>();
        name.text = player.name;
        name.onValueChanged.AddListener(delegate { player.name = name.text; });

        // prepares playerGameObject and returns
        playerObject.transform.SetParent(transform, false);
        playerObject.GetComponent<RectTransform>().position = new Vector3(playerObject.GetComponent<RectTransform>().position.x, transform.GetChild(2).GetComponent<RectTransform>().position.y - (0.3649f * Screen.height * (transform.childCount - 3)), 0);
        playerObject.SetActive(true);
        return playerObject;
    }
}
