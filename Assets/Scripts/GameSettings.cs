using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine.SceneManagement;
using System.Reflection;
using SFB;

public static class GameSettings
{
    public static string MapData { get; set; } = Resources.Load<TextAsset>("Maps/Facility381").text;

    public static int CountdownLength { get; set; } = 10;

    public static Player[] Players { get; set; } = new Player[] { new Player("Player"), new Player("CPU1", true) };

    public static Disease[] Diseases { get; set; } = new Disease[] { new Disease("C-394", typeof(ClassicDisease)) };

    public static Controls Controls { get; set; } = new Controls();

    public static List<InfectionData> GameStats { get; set; } = new List<InfectionData>();

    public static void DownloadGameSettings(bool askPath = true)
    {
        string path;

        if (askPath)
        {
            // opens save dialogue
            path = StandaloneFileBrowser.SaveFilePanel(
            "Save Game Setup",
            "",
            "saved setup",
            new[] { new ExtensionFilter("Project Malady Game Setup", "gameSetup") });
        }
        else
        {
            path = Application.persistentDataPath + "/savedSetup.gameSetup";
        }

        // makes sure save wasn't cancelled
        if (path.Length != 0)
        {
            // deletes any file in path to replace it
            File.Delete(path);

            // serializes object
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GameSettingsSerializable));
            MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, new GameSettingsSerializable());
            memoryStream.Position = 0;

            StreamReader streamReader = new StreamReader(memoryStream);

            // translates stream to string
            string json = streamReader.ReadToEnd();

            // closes streams
            memoryStream.Close();
            streamReader.Close();

            // opens file path
            StreamWriter writer = new StreamWriter(path, true);

            // serializes GameSettings as JSON and writes it in file path
            writer.WriteLine(json);
            writer.Close();
        }
    }

    public static void LoadGameSettings(bool askPath = true)
    {
        string path = "";

        if (askPath)
        {
            // opens load dialogue
            string[] pathArray = StandaloneFileBrowser.OpenFilePanel(
            "Load Game Setup",
            "",
            new[] { new ExtensionFilter("Project Malady Game Setup", "gameSetup") }
            , false);

            if(pathArray.Length == 1)
            {
                path = pathArray[0];
            }
        }
        else
        {
            path = Application.persistentDataPath + "/savedSetup.gameSetup";
        }

        // makes sure load wasn't cancelled
        if (path.Length != 0 && File.Exists(path))
        {
            GameSettingsSerializable loadedSettings;

            // reads file path
            StreamReader reader = new StreamReader(path, true);
            string loadedText = reader.ReadLine();
            reader.Close();

            // deserializes file path's contents
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(GameSettingsSerializable));
            MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(loadedText));

            // sets loaded settings to parsed file
            loadedSettings = (GameSettingsSerializable)deserializer.ReadObject(memoryStream);

            // closes stream
            memoryStream.Close();

            // sets GameSettings to loadedSettings
            GameSettings.MapData = loadedSettings.MapData;
            GameSettings.CountdownLength = loadedSettings.CountdownLength;
            GameSettings.Players = loadedSettings.Players;
            GameSettings.Diseases = loadedSettings.Diseases;
            GameSettings.Controls = loadedSettings.Controls;

            // converts strings to keycodes in controls
            foreach (FieldInfo fieldInfo in typeof(Controls).GetFields())
            {
                string currentField = (string)fieldInfo.GetValue(loadedSettings.Controls);

                // replaces keycode with name
                if (currentField.Contains("keycode:"))
                {
                    Debug.Log(currentField.Remove(0, "keycode:".Length));
                    KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), currentField.Remove(0, "keycode:".Length));
                    fieldInfo.SetValue(GameSettings.Controls, keyCode);
                }
            }

            for (int i = 0; i < GameSettings.Diseases.Length; i++)
            {
                GameSettings.Diseases[i].type = Type.GetType(loadedSettings.Diseases[i].typeString);
            }

            // reloads scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [Serializable]
    class GameSettingsSerializable
    {
        public string MapData = GameSettings.MapData;
        public int CountdownLength = GameSettings.CountdownLength;
        public Player[] Players = GameSettings.Players;
        public Disease[] Diseases = GameSettings.Diseases;
        public Controls Controls = GameSettings.Controls;

        public GameSettingsSerializable()
        {
            foreach(FieldInfo fieldInfo in typeof(Controls).GetFields())
            {
                object currentField = fieldInfo.GetValue(Controls);

                // replaces keycode with name
                if(currentField.GetType() == typeof(KeyCode))
                {
                    fieldInfo.SetValue(Controls, "keycode:" + currentField.ToString());
                }
            }
        }
    }
}

[Serializable]
public class Player
{
    public string name;
    public bool bot;
    public bool spectator;
    [NonSerialized] public GameObject gameObject;
    public Disease disease = null;
    public bool assignedSpecifically = false;

    public Player(string nameInput, bool botInput = false, bool spectatorInput = false, bool assignedSpecificallyInput = false, Disease diseaseInput = null, GameObject gameObjectInput = null)
    {
        name = nameInput;
        bot = botInput;
        spectator = spectatorInput;
        assignedSpecifically = assignedSpecificallyInput;
        disease = diseaseInput;
        gameObject = gameObjectInput;
    }
}

[Serializable]
public class Disease
{
    public string name;
    public string typeString = typeof(ClassicDisease).FullName;
    public DiseaseParameter[] parameters = null;
    public int index;
    public bool assignedSpecifically = false;


    public Type type
    {
        get
        {
            return Type.GetType(typeString);
        }

        set
        {
            typeString = value.FullName;
        }
    }

    public Disease(string nameInput, Type typeInput, DiseaseParameter[] parametersInput = null, int indexInput = 0, bool assignedSpecificallyInput = false)
    {
        name = nameInput;
        type = typeInput == null ? typeof(ClassicDisease) : typeInput;
        parameters = parametersInput;
        index = indexInput;
        assignedSpecifically = assignedSpecificallyInput;
    }
}

[Serializable]
public class DiseaseParameter
{
    public string key;
    public object value; // object so it can have any data type
    public string readableName;

    public DiseaseParameter(string keyInput, object valueInput, string readableNameInput = null)
    {
        key = keyInput;
        readableName = readableNameInput;
        value = valueInput;
    }
}

public class Controls
{
    // object so it can have any data type (keycode or string)
    public object up = "w";
    public object left = "a";
    public object down = "s";
    public object right = "d";
    public object pause = KeyCode.Escape;
}


public class InfectionData
{
    public GameObject alpha;
    public int infectionCount;
    public int killCount;
    public Disease disease;

    public InfectionData(GameObject alphaInput, int infectionCountInput, int killCountInput, Disease diseaseInput)
    {
        alpha = alphaInput;
        killCount = killCountInput;
        infectionCount = infectionCountInput;
        disease = diseaseInput;
    }
}