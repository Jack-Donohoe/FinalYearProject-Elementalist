using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void GoToMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
    
    public void RestartGame()
    {
        GameManager.Instance.LoadLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
