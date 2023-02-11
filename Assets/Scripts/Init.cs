using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    void Start()
    {
        GameSettings.LoadGameSettings(false);
        SceneManager.LoadScene("Menu");
    }
}
