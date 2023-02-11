using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float playerSpeed = 350f;
    public bool spectatorMode = false;
    GameObject mainCamera;
    float spectatorSpeed = 19.425f;
    public Vector3 spawnPoint;
    public bool gameStarted = false;

    public Sprite healthySprite;
    public Sprite infectedSprite;
    public Sprite deadSprite;

    public GameObject playerTemplate;
    public GameObject spectatorTemplate;
    public GameObject CPUTemplate;
    public GameObject playerNameTemplate;

    public AstarPath astarPath;

    public GameObject pauseMenu;
    public bool inPauseAnimation = false;
    bool pauseDown = false;

    public GameObject deathMenu;
    public GameObject gameOverMenu;
    public GameObject settingsMenu;

    public Text countdownText;
    public Text messageText;
    int messageCount;

    float countdown;
    bool countingDown;

    void Start()
    {
        // resets time scale
        Time.timeScale = 1;

        // resets game stats
        GameSettings.GameStats = new List<InfectionData>();

        // checks if in game
        if (SceneManager.GetActiveScene().name == "InGame")
        {
            // generates map from game settings
            FindObjectOfType<MapFiles>().JSONToMap(GameSettings.MapData);

            astarPath.Scan();

            // sets countdown time
            countdown = GameSettings.CountdownLength;
            countingDown = true;

            // adds randomly assigned diseases
            if (GameSettings.Players.Length > 0)
            {
                // shuffles disease list
                List<Disease> diseaseList = new List<Disease>(GameSettings.Diseases);
                Disease[] randomDiseases = new Disease[GameSettings.Players.Length];

                for (; diseaseList.Count < GameSettings.Players.Length; diseaseList.Add(null)) { };

                int diseaseIndex = 0;
                // loops through array and swaps items to shuffle
                for (int i = 0; i < GameSettings.Players.Length; i++)
                {
                    // skips player if already assigned a disease or if spectator
                    if (GameSettings.Players[i].assignedSpecifically && GameSettings.Players[i].disease != null) {
                        Debug.Log(GameSettings.Players[i].disease);
                        GameSettings.Diseases[GameSettings.Players[i].disease.index].assignedSpecifically = true;
                        continue;
                    }
                    if (GameSettings.Players[i].spectator) continue;

                    // gets random item to insert
                    int randomIndex = Random.Range(0, diseaseList.Count);

                    randomDiseases[diseaseIndex] = diseaseList[randomIndex];

                    // removes from old list
                    diseaseList.RemoveAt(randomIndex);

                    diseaseIndex++;
                }

                // loops through players who are not assigned disease already or are spectators
                diseaseIndex = 0;
                for (int i = 0; i < GameSettings.Players.Length; i++, diseaseIndex++)
                {
                    // skips player if already assigned a disease or if spectator or if disease already assigned
                    if (GameSettings.Players[i].assignedSpecifically) continue;
                    if (randomDiseases[diseaseIndex] != null && randomDiseases[diseaseIndex].assignedSpecifically) continue;
                    if (GameSettings.Players[i].spectator) continue;

                    // ends loop if no more diseases are left to hand out
                    if (diseaseIndex == randomDiseases.Length) break;

                    if (randomDiseases[diseaseIndex] != null)
                    {
                        GameSettings.Players[i].disease = randomDiseases[diseaseIndex];
                    }
                }
            }

            // creates players
            for (int i = 0; i < GameSettings.Players.Length; i++)
            {
                GameObject player;

                // checks if player is bot
                if (GameSettings.Players[i].bot)
                {
                    player = Instantiate(CPUTemplate);
                }
                else
                {
                    // if not spectator
                    if (GameSettings.Players[i].spectator == false)
                    {
                        player = Instantiate(playerTemplate);
                    }
                    else // if spectator
                    {
                        player = Instantiate(spectatorTemplate);
                        FindObjectOfType<GameManager>().SpectatorMode(player);
                    }
                }

                player.name = GameSettings.Players[i].name;
                player.transform.SetParent(GameObject.Find("Players").transform);
                player.SetActive(true);

                // if not spectator
                if (GameSettings.Players[i].spectator == false)
                {
                    // sets name component
                    player.gameObject.GetComponent<PlayerName>().playerName = player.name;

                    GameSettings.Players[i].gameObject = player;
                }
            }
        }
    }

    void Update()
    {
        // pause
        if(SceneManager.GetActiveScene().name == "InGame")
        {
            if (GameSettings.Controls.pause.GetType() == typeof(string))
            {
                if (Input.GetKey((string)GameSettings.Controls.pause))
                {
                    pauseDown = true;
                    Pause();
                }
                else
                {
                    pauseDown = false;
                }
            }
            else // if keycode
            {
                if (Input.GetKey((KeyCode)GameSettings.Controls.pause))
                {
                    pauseDown = true;
                    Pause();
                }
                else
                {
                    pauseDown = false;
                }
            }
        }

        // spectator mode controls
        if (spectatorMode)
        {
            // controls

            // up

            // if string
            if (GameSettings.Controls.up.GetType() == typeof(string))
            {
                if (Input.GetKey((string)GameSettings.Controls.up))
                {
                    mainCamera.transform.position += new Vector3(0, spectatorSpeed * Time.deltaTime, 0);
                }
            }
            else // if keycode
            {
                if (Input.GetKey((KeyCode)GameSettings.Controls.up))
                {
                    mainCamera.transform.position += new Vector3(0, spectatorSpeed * Time.deltaTime, 0);
                }
            }

            // left

            // if string
            if (GameSettings.Controls.left.GetType() == typeof(string))
            {
                if (Input.GetKey((string)GameSettings.Controls.left))
                {
                    mainCamera.transform.position += new Vector3(-spectatorSpeed * Time.deltaTime, 0, 0);
                }
            }
            else // if keycode
            {
                if (Input.GetKey((KeyCode)GameSettings.Controls.left))
                {
                    mainCamera.transform.position += new Vector3(-spectatorSpeed * Time.deltaTime, 0, 0);
                }
            }

            // down

            // if string
            if (GameSettings.Controls.down.GetType() == typeof(string))
            {
                if (Input.GetKey((string)GameSettings.Controls.down))
                {
                    mainCamera.transform.position += new Vector3(0, -spectatorSpeed * Time.deltaTime, 0);
                }
            }
            else // if keycode
            {
                if (Input.GetKey((KeyCode)GameSettings.Controls.down))
                {
                    mainCamera.transform.position += new Vector3(0, -spectatorSpeed * Time.deltaTime, 0);
                }
            }

            // right

            // if string
            if (GameSettings.Controls.right.GetType() == typeof(string))
            {
                if (Input.GetKey((string)GameSettings.Controls.right))
                {
                    mainCamera.transform.position += new Vector3(spectatorSpeed * Time.deltaTime, 0, 0);
                }
            }
            else // if keycode
            {
                if (Input.GetKey((KeyCode)GameSettings.Controls.right))
                {
                    mainCamera.transform.position += new Vector3(spectatorSpeed * Time.deltaTime, 0, 0);
                }
            }
        }

        // counts down towards infection
        if (countingDown)
        {
            // infection not yet released
            if (countdown > 0)
            {
                countdown -= Time.deltaTime;
                countdownText.text = $"{System.Math.Round(countdown)} seconds until infection.";
            }
            else if (countdown > -2 && gameStarted == false)
            {
                countingDown = false;
                gameStarted = true;

                countdownText.text = "The infection is now loose.";
                countdownText.GetComponent<Animator>().Play("TextFade");

                // applies disease script to each player
                for (int i = 0; i < GameSettings.Players.Length; i++)
                {
                    // skips over healthy (null)
                    if (GameSettings.Players[i].disease == null) continue;

                    // applies disease to player
                    Component component = GameSettings.Players[i].gameObject.AddComponent(GameSettings.Players[i].disease.type);

                    // sets disease of component
                    FieldInfo diseaseFieldInfo = GameSettings.Players[i].disease.type.GetField("disease");
                    diseaseFieldInfo.SetValue(component, GameSettings.Players[i].disease);

                    // applies parameters to player disease
                    if (GameSettings.Players[i].disease.parameters != null)
                    {
                        for (int j = 0; j < GameSettings.Players[i].disease.parameters.Length; j++)
                        {
                            FieldInfo fieldInfo = GameSettings.Players[i].gameObject.GetComponent(GameSettings.Players[i].disease.type).GetType().GetField(GameSettings.Players[i].disease.parameters[j].key);
                            fieldInfo.SetValue(GameSettings.Players[i].gameObject.GetComponent(GameSettings.Players[i].disease.type), GameSettings.Players[i].disease.parameters[j].value);
                        }
                    }

                    // replaces HealthyAI with InfectedAI if needed
                    if (GameSettings.Players[i].bot && GameSettings.Players[i].disease.type != typeof(Asymptomatic))
                    {
                        Destroy(GameSettings.Players[i].gameObject.GetComponent<HealthyAI>());
                        GameSettings.Players[i].gameObject.AddComponent<InfectedAI>();
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        GameSettings.DownloadGameSettings(false);
    }

    public void SpectatorMode(GameObject cameraInput)
    {
        mainCamera = cameraInput;
        mainCamera.transform.SetParent(null);
        spectatorMode = true;

        ShowMessage("Spectator mode is active. You can observe the game.");
    }

    public void SpectatorModeButton(bool unpause)
    {
        // gets player data
        GameObject player = FindObjectsOfType<HumanPlayer>()[0].gameObject;

        if (player.GetComponent<PlayerMovement>())
        {
            // changes sprite to dead sprite
            player.GetComponent<SpriteRenderer>().sprite = deadSprite;
            player.tag = "Dead";
        }

        // enables spectator 
        ShowMessage("Spectator mode is active. You can observe the game.");
        SpectatorMode(player.transform.Find("Main Camera").gameObject);

        if (unpause)
        {
            Pause();
        }
        else
        {
            deathMenu.transform.GetChild(0).GetComponent<Animator>().Play("FolderExitHorizontal");
        }
    }

    public void Pause()
    {
        StartCoroutine(TimedPause());
    }

    IEnumerator TimedPause()
    {
        if(inPauseAnimation == false)
        {
            if (Time.timeScale == 1)
            {
                pauseMenu.SetActive(true);
                pauseMenu.transform.GetChild(0).GetComponent<Animator>().Play("FolderEnter");

                inPauseAnimation = true;
                Time.timeScale = 0;

                while (pauseDown)
                {
                    yield return null;
                }

                inPauseAnimation = false;
            }
            else
            {
                pauseMenu.transform.GetChild(0).GetComponent<Animator>().Play("FolderExitHorizontal");

                inPauseAnimation = true;

                while (pauseDown)
                {
                    yield return null;
                }

                Time.timeScale = 1;
                inPauseAnimation = false;
            }
        }        
    }

    public void ShowDeathMenu(string description)
    {
        StartCoroutine(DeathMenuIEnumerator(1.5f));

        IEnumerator DeathMenuIEnumerator(float delay)
        {
            yield return new WaitForSeconds(delay);
            // shows menu with description
            deathMenu.transform.Find("Content").Find("Description").GetComponent<Text>().text = description;
            deathMenu.SetActive(true);
        }
    }

    public void ShowMessage(string message, int duration = 3)
    {
        messageText.text = message;

        messageCount++;
        int messageIndex = messageCount;

        // resets color
        messageText.gameObject.GetComponent<Animator>().Play("Empty");
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1);

        StartCoroutine(HideMessage());

        IEnumerator HideMessage()
        {
            // waits for duration
            yield return new WaitForSeconds(duration);

            if (messageCount == messageIndex)
            {
                messageText.gameObject.GetComponent<Animator>().Play("TextFade");
            }
        }
    }

    public void ShowSettingsMenu()
    {
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<Animator>().Play("FolderEnter");
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.GetComponent<Animator>().Play("FolderExitHorizontal");
    }

    public bool CountInfections(GameObject alpha, Disease disease)
    {
        // finds if infection data already exists
        bool alreadyExists = false;

        foreach (InfectionData currentInfection in GameSettings.GameStats)
        {
            if(ReferenceEquals(currentInfection.alpha, alpha))
            {
                currentInfection.infectionCount++;
                alreadyExists = true;
                break;
            }
        }
        
        if (alreadyExists == false)
        {
            InfectionData newInfectionData = new InfectionData(alpha, 1, 0, disease);
            GameSettings.GameStats.Add(newInfectionData);
        }
        
        // no healthy people left
        if(GameObject.FindGameObjectsWithTag("Healthy").Length == 0)
        {
            gameOverMenu.SetActive(true);
            gameOverMenu.transform.GetChild(0).Find("Description").GetComponent<Text>().text = $"No healthy players are left.";
            Time.timeScale = 0;

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CountKills(GameObject alpha, Disease disease)
    {
        // finds if infection data already exists
        bool alreadyExists = false;

        foreach (InfectionData currentInfection in GameSettings.GameStats)
        {
            if (ReferenceEquals(currentInfection.alpha, alpha))
            {
                currentInfection.killCount++;
                alreadyExists = true;
                break;
            }
        }

        if (alreadyExists == false)
        {
            InfectionData newInfectionData = new InfectionData(alpha, 1, 1, disease);
            GameSettings.GameStats.Add(newInfectionData);
        }

        // no healthy people left
        if (GameObject.FindGameObjectsWithTag("Infected").Length == 0)
        {
            gameOverMenu.SetActive(true);
            gameOverMenu.transform.GetChild(0).Find("Description").GetComponent<Text>().text = $"No infected players are left alive.";
            Time.timeScale = 0;

            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadSetup()
    {
        GameSettings.LoadGameSettings();
    }

    public void SaveSetup()
    {
        GameSettings.DownloadGameSettings();
    }

    public void OpenGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
    }

    public void OpenMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
    public void OpenCredits()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Credits");
    }

    public void OpenSettings()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Settings");
    }

    public void OpenGameSettings()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameSettings");
    }
}