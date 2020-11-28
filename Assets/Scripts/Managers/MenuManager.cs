using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public void LoadIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void LoadStage()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
