using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Canvas pauseMenu;
    public AudioClip buttonHovered;
    public AudioClip buttonPressed; //Not used

    [Header("Ability UI")]
    public Image moonshotImg;
    public Image tailAttackImg;
    public TextMeshProUGUI moonshotText;
    public TextMeshProUGUI tailAttackText;
    public Color disabledColor;
    public Color enabledColor;

    [Header("Tutorial On Screen")]
    public float fadeTime;
    public float textTTL;
    public Image bgTutorial;
    public TextMeshProUGUI tutorialText;

    /*
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Color winColorPanel;
    public Color loseColorPanel;
    [SerializeField] string winText;
    [SerializeField] string loseText;
    */

    AudioSource aSource;
    string[] tutorial;
    bool[] printed;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        pauseMenu.enabled = false;
        aSource = GetComponent<AudioSource>();
        Time.timeScale = 1;

        moonshotText.enabled = false;
        moonshotImg.color = disabledColor;
        tailAttackText.enabled = false;
        tailAttackImg.color = disabledColor;

        bgTutorial.enabled = false;
        tutorialText.text = "";

        //gameOverPanel.SetActive(false);

        #region tutorial text
        tutorial = new string[6];
        tutorial[0] = "Move with WASD";
        tutorial[1] = "Attack using Mouse SX. Eliminate enemies as fast as possible, Alklha is eating the moon! If the moon disappear, you will lose";
        tutorial[2] = "You have just picked up a healer crystal. This will provides you some usefull energy";
        tutorial[3] = "You have just picked up a fire crystal. Now you can use your tale as a weapon! Press 'E' key!";
        tutorial[4] = "The Moonshot is ready! Unleashed it pressing the 'Q' key!";
        tutorial[5] = "Alklha is chasing you! Damage it as much as possible to restore part of the moon";
        #endregion

        printed = new bool[tutorial.Length];
        for(int i=0; i < printed.Length; i++)
        {
            printed[i] = false;
        }

        StartCoroutine(PrintOnScreen(tutorial, 2));
        printed[0] = true;
        printed[1] = true;
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

    public void EnableMoonshotUI()
    {
        moonshotText.enabled = true;
        moonshotImg.color = enabledColor;
    }

    public void DisableMoonshotUI()
    {
        moonshotText.enabled = false;
        moonshotImg.color = disabledColor;
    }

    public void EnableTailAttackUI()
    {
        tailAttackText.enabled = true;
        tailAttackImg.color = enabledColor;
    }

    public void DisableTailAttackUI()
    {
        tailAttackText.enabled = false;
        tailAttackImg.color = disabledColor;
    }

    public void PrintText(int index)
    {
        if (!printed[index])
        {
            printed[index] = true;
            StartCoroutine(PrintOnScreen(tutorial[index]));
        }
    }

    IEnumerator PrintOnScreen(string s)
    {
        Color originalColor = enabledColor;

        bgTutorial.enabled = true;

        for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
        {
            tutorialText.text = s;
            tutorialText.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeTime));
            yield return null;
        }

        yield return new WaitForSeconds(textTTL);

        for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
        {
            tutorialText.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeTime));
            yield return null;
        }

        tutorialText.text = "";

        bgTutorial.enabled = false;
    }

    IEnumerator PrintOnScreen(string []s, int range)
    {
        Color originalColor = enabledColor;
        int counter = 0;

        bgTutorial.enabled = true;

        while(counter < s.Length && counter < range)
        {
            for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
            {
                tutorialText.text = s[counter];
                tutorialText.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeTime));
                yield return null;
            }

            yield return new WaitForSeconds(textTTL);

            for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
            {
                tutorialText.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeTime));
                yield return null;
            }

            counter++;
        }
        tutorialText.text = "";

        bgTutorial.enabled = false;
    }
    
    /*
    public void OnGameOver(bool res)
    {
        Debug.Log("Gameover UI");
        gameOverPanel.SetActive(true);

        if (res)    //player has won
        {
            gameOverPanel.GetComponent<Image>().color = winColorPanel;
            gameOverText.text = winText;
        }
        else        //player has lost
        {
            gameOverPanel.GetComponent<Image>().color = loseColorPanel;
            gameOverText.text = loseText;
        }
    }
    */

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ReloadStage()
    {
        SceneManager.LoadScene("GameScene");
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

}
