using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void OpenAbout()
    {
        SceneManager.LoadScene("AboutScene"); 
    }

    public void OpenTheme()
    {
        SceneManager.LoadScene("ThemeScene"); 
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

