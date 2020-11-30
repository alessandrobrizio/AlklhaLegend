using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject alklha;

    private void Start()
    {
        int lastPlayResult = PlayerPrefs.GetInt("LastPlayResult");
        player.SetActive(lastPlayResult == 1);
        alklha.SetActive(lastPlayResult == 0);
    }

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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
