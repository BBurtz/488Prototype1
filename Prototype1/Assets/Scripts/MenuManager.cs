using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject PauseMenu;

    public void UnPause()
    {
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
        print("WORKS");
    }

    public void Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LevelButtonPressed(int level)
    {
        SceneManager.LoadScene(level);
    }
}
