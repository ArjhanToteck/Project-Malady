using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiseaseSettings : MonoBehaviour
{
    public GameObject eventSystem;

    public InputField countdownInput;

    public GameObject diseaseTemplate;
    public GameObject parameterTemplate;

    void Start()
    {
        // keeps track of countdownInput
        countdownInput.onValueChanged.AddListener(delegate { ChangeCountdown(); });
        countdownInput.text = GameSettings.CountdownLength.ToString();

        // converts all existing diseases to GameObjects
        for (int i = 0; i < GameSettings.Diseases.Length; i++)
        {
            // makes sure index is included
            GameSettings.Diseases[i].index = i;

            // converts disease to object
            DiseaseToObject(i);

            // resizes scrollview if neccessary
            if (i > 0)
            {
                // resizes scrollview
                transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 165);
            }
        }
    }

    GameObject DiseaseToObject(int diseaseIndex, GameObject duplicate = null)
    {
        GameObject diseaseObject;
        Disease disease = GameSettings.Diseases[diseaseIndex];

        if (duplicate == null)
        {
            diseaseObject = Instantiate(diseaseTemplate);
        }
        else
        {
            diseaseObject = Instantiate(duplicate);
        }

        // duplicate
        diseaseObject.transform.Find("Duplicate").GetComponent<Button>().onClick.AddListener(delegate {
            // deselects button
            eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

            // copies disease data over to duplicate
            Stream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, disease);
            stream.Seek(0, SeekOrigin.Begin);
            Disease clonedDisease = (Disease)formatter.Deserialize(stream);

            // converts GameSettings.Diseases to list to easily add clonedDisease
            List<Disease> newDiseases = new List<Disease>(GameSettings.Diseases);

            // sets index
            clonedDisease.index = GameSettings.Diseases.Length;
            newDiseases.Add(clonedDisease);

            // adds duplicated disease to gamesettings.diseases
            GameSettings.Diseases = newDiseases.ToArray();

            // sets disease once again to avoid problems
            disease = GameSettings.Diseases[diseaseObject.transform.GetSiblingIndex() - 1];

            // creates object from duplicated disease and sets delete to active
            DiseaseToObject(transform.childCount - 1, diseaseObject).transform.Find("Delete").gameObject.SetActive(true);
            diseaseObject.transform.Find("Delete").gameObject.SetActive(true);

            // resizes scrollview
            transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 165);
        });

        // delete
        diseaseObject.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(delegate {
            // deselects button
            eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);

            // resizes gamesettings.diseases to remove disease
            List<Disease> newDiseases = new List<Disease>(GameSettings.Diseases);
            newDiseases.RemoveAt(newDiseases.Count - 1);
            GameSettings.Diseases = newDiseases.ToArray();

            // repositions siblings that are below
            for (int i = 0; i < transform.childCount - diseaseObject.transform.GetSiblingIndex(); i++)
            {
                // gets sibling
                RectTransform sibling = transform.GetChild(i + diseaseObject.transform.GetSiblingIndex()).GetComponent<RectTransform>();

                // repositions sibling
                sibling.position += new Vector3(0, 0.3649f * Screen.height, 0);
            }

            // removes delete button if neccessary
            if (GameSettings.Diseases.Length == 1)
            {
                transform.GetChild(1).Find("Delete").gameObject.SetActive(false);
                transform.GetChild(2).Find("Delete").gameObject.SetActive(false);
            }

            // destroys gameobject
            Destroy(diseaseObject);
            
            // resizes scrollview
            transform.parent.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 165);
        });

        // disease name
        InputField name = diseaseObject.transform.Find("Name").Find("InputField").GetComponent<InputField>();
        name.text = disease.name;
        name.onValueChanged.AddListener(delegate { disease.name = name.text; });

        // sets type
        Dropdown type = diseaseObject.transform.Find("Type").Find("Dropdown").GetComponent<Dropdown>();
        SelectType();
        OnTypeChanged(false);

        void SelectType()
        {
            string diseaseName = "";
            int optionIndex = 0;

            // loops through diseases
            foreach (PropertyInfo propertyInfo in typeof(Diseases).GetProperties())
            {
                DiseaseInfo value = (DiseaseInfo)propertyInfo.GetValue(null);

                // checks if types match
                if (value.type == disease.type)
                {
                    Debug.Log(disease.type);
                    // sets diseaseName to name of property
                    diseaseName = propertyInfo.Name;
                    break;
                }
            }

            for (int i = 0; i < type.options.Count; i++)
            {
                if(type.options[i].text == diseaseName)
                {
                    optionIndex = i;
                    break;
                }
            }

            type.value = optionIndex;
        }

        // on type changed
        void OnTypeChanged(bool realChange = true)
        {
            Transform parameters = diseaseObject.transform.Find("Parameters");

            // destroys all parameters if applicable
            for (int i = 0; i < parameters.childCount; i++)
            {
                Destroy(parameters.GetChild(i).gameObject);
            }

            // get info of selected property
            PropertyInfo propertyInfo = typeof(Diseases).GetProperty(type.options[type.value].text);
            DiseaseInfo selection = (DiseaseInfo)propertyInfo.GetValue(null);

            // applies type and default parameters to disease
            disease.type = selection.type;
            if (realChange || (disease.parameters == null && selection.parameters != null))
            {
                if (selection.parameters == null)
                {
                    disease.parameters = null;
                }
                else
                {
                    disease.parameters = (DiseaseParameter[])selection.parameters.Clone();
                }
            }

            // checks if parameters exist
            if (disease.parameters != null)
            {
                for (int i = 0; i < disease.parameters.Length; i++)
                {
                    // gets selected parameter
                    DiseaseParameter currentParameter = disease.parameters[i];

                    // creates parameter from template
                    GameObject currentParameterObject = Instantiate(parameterTemplate);

                    // sets parent so in canvas environment
                    currentParameterObject.transform.SetParent(parameters, false);

                    GameObject textTransform = currentParameterObject.transform.Find("Text").gameObject;

                    // changes text to readable name
                    textTransform.GetComponent<Text>().text = currentParameter.readableName + ":";

                    // adjusts size of text to align toggle
                    float offset = 65f;
                    textTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(textTransform.GetComponent<Text>().preferredWidth + offset, textTransform.GetComponent<RectTransform>().sizeDelta.y);
                    StartCoroutine(ResizeParameter());

                    // positions parameter object
                    currentParameterObject.GetComponent<RectTransform>().localPosition = new Vector3(-243, -40 - (i * 32.5f), 0);

                    // enables input depending on type
                    if (currentParameter.value.GetType() == typeof(bool))
                    {
                        // prepares toggle
                        currentParameterObject.transform.Find("IntInput").gameObject.SetActive(false);

                        Toggle toggle = currentParameterObject.transform.Find("Toggle").GetComponent<Toggle>();
                        toggle.isOn = (bool)currentParameter.value;

                        // sets value to toggle value
                        int eventListenerI = i;
                        toggle.onValueChanged.AddListener(delegate { disease.parameters[eventListenerI].value = toggle.isOn; });
                    }
                    else
                    {
                        // prepares input field
                        currentParameterObject.transform.Find("Toggle").gameObject.SetActive(false);

                        InputField inputField = currentParameterObject.transform.Find("IntInput").Find("InputField").GetComponent<InputField>();
                        inputField.text = currentParameter.value.ToString();

                        // sets value to input field value
                        int eventListenerI = i;
                        inputField.onValueChanged.AddListener(delegate {
                            try
                            {
                                disease.parameters[eventListenerI].value = int.Parse(inputField.text);
                            }
                            catch
                            {
                                disease.parameters[eventListenerI].value = 0;
                            }
                        });
                    }

                    IEnumerator ResizeParameter()
                    {

                        // wait one frame
                        yield return 0;

                        // resizes texttransform
                        textTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(textTransform.GetComponent<Text>().preferredWidth + offset, textTransform.GetComponent<RectTransform>().sizeDelta.y);
                    }
                }
            }
        }

        // sets event listener
        type.onValueChanged.AddListener(delegate {
            OnTypeChanged();
        });

        // prepares diseaseGameObject and returns
        diseaseObject.transform.SetParent(transform, false);
        diseaseObject.GetComponent<RectTransform>().position = new Vector3(diseaseObject.GetComponent<RectTransform>().position.x, transform.GetChild(1).GetComponent<RectTransform>().position.y - (0.3649f * Screen.height * (transform.childCount - 2)), 0);
        diseaseObject.SetActive(true);
        return diseaseObject;
    }

    public void ChangeCountdown()
    {
        try
        {
            GameSettings.CountdownLength = int.Parse(countdownInput.text);
        }
        catch
        {
            GameSettings.CountdownLength = 0;
        }
    }

    static class Diseases
    {
        public static DiseaseInfo Asymptomatic {
            get
            {
                DiseaseInfo diseaseInfo = new DiseaseInfo();
                diseaseInfo.type = typeof(Asymptomatic);
                diseaseInfo.parameters = new DiseaseParameter[] { new DiseaseParameter("infectionTimer", 10, "Infection Delay"), new DiseaseParameter("lethal", false, "Lethal") };

                return diseaseInfo;
            }
        }

        public static DiseaseInfo Classic
        {
            get
            {
                DiseaseInfo diseaseInfo = new DiseaseInfo();
                diseaseInfo.type = typeof(ClassicDisease);
                diseaseInfo.parameters = new DiseaseParameter[] { new DiseaseParameter("lethal", false, "Lethal") };
                return diseaseInfo;
            }
        }

        public static DiseaseInfo Hivemind
        {
            get
            {
                DiseaseInfo diseaseInfo = new DiseaseInfo();
                diseaseInfo.type = typeof(Hivemind);
                diseaseInfo.parameters = null; // no parameters, so null
                return diseaseInfo;
            }
        }

        public static DiseaseInfo Parasite
        {
            get
            {
                DiseaseInfo diseaseInfo = new DiseaseInfo();
                diseaseInfo.type = typeof(Parasite);
                diseaseInfo.parameters = new DiseaseParameter[] { new DiseaseParameter("deathTimerMax", 25, "Death Timer") };
                return diseaseInfo;
            }
        }
    }

    class DiseaseInfo
    {
        public Type type;
        public DiseaseParameter[] parameters;
    }
}