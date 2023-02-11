using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public Animator animator;
    public GameObject folderExited;

    public void Play()
    {
        // starts animation
        animator.SetTrigger("FolderExit");
        StartCoroutine(PlayIEnumerator());
    }

    IEnumerator PlayIEnumerator()
    {
        // waits for animation end
        while (folderExited.activeInHierarchy == false)
        {
            yield return null;
        }

        // opens game settings
        SceneManager.LoadScene("GameSettings");
    }

    public void Credits()
    {
        // starts animation
        animator.SetTrigger("FolderExit");
        StartCoroutine(CreditsIEnumerator());
    }

    IEnumerator CreditsIEnumerator()
    {
        // waits for animation end
        while (folderExited.activeInHierarchy == false)
        {
            yield return null;
        }

        // opens game settings
        SceneManager.LoadScene("Credits");
    }

    public void Settings()
    {
        // starts animation
        animator.SetTrigger("FolderExit");
        StartCoroutine(SettingsIEnumerator());
    }

    IEnumerator SettingsIEnumerator()
    {
        // waits for animation end
        while (folderExited.activeInHierarchy == false)
        {
            yield return null;
        }

        // opens game settings
        SceneManager.LoadScene("Settings");
    }

    public void Exit()
    {
        // starts animation
        animator.SetTrigger("FolderExit");
        StartCoroutine(ExitIEnumerator());
    }

    IEnumerator ExitIEnumerator()
    {
        // waits for animation end
        while (folderExited.activeInHierarchy == false)
        {
            yield return null;
        }

        // closes game
        Application.Quit();
    }
}
