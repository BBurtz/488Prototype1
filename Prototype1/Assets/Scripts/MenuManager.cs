using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
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
        SceneManager.LoadScene(0);
    }

    public void LevelButtonPressed(int level)
    {
        SceneManager.LoadScene(level);
    }
}
