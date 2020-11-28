using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public AudioClip buttonHovered;
    public AudioClip buttonPressed;

    AudioSource aSource;

    private void Start()
    {
        pauseMenu.enabled = false;
        aSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OnButtonHover()
    {
        aSource.PlayOneShot(buttonHovered);
    }

    public void OnButtonPressed()
    {
        aSource.PlayOneShot(buttonPressed);
    }

    public void ResumeGame()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnButtonPressed();

            pauseMenu.enabled = !pauseMenu.enabled;

            if (pauseMenu.enabled)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}
