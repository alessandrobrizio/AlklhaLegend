using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum TutorialAction
{
    Goal,
    MoveInstructions,
    BasicAttackInstructions,
    EnergyPowerUpCollected,
    ElementalAttackCollected,
    MoonshotReadyInstructions,
    RestoreMoon,
    EnergyInstructions
}

[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Canvas pauseMenu = null;
    public AudioClip buttonHovered = null;
    public AudioClip buttonPressed = null; //Not used
    public GameObject pauseTitle = null;
    public Button resumeGameButton = null;
    public TextMeshProUGUI endGameText = null;

    [Header("Ability UI")]
    public Image moonshotImg = null;
    public Image tailAttackImg = null;
    public TextMeshProUGUI moonshotText = null;
    public TextMeshProUGUI tailAttackText = null;
    public Color disabledColor;
    public Color enabledColor;

    [Header("Tutorial On Screen")]
    public float fadeTime;
    public float textTTL;
    public Image bgTutorial = null;
    public TextMeshProUGUI tutorialText = null;

    private AudioSource aSource = null;
    private Dictionary<TutorialAction, string> tutorial = null;
    private bool[] printed;
    private Queue<TutorialAction> outputInstructionsQueue = new Queue<TutorialAction>();
    private Coroutine instructionsCoroutine = null;

    private bool gameEnded = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Debug.Log("UI Manager start");
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
        tutorial = new Dictionary<TutorialAction, string> {
            { TutorialAction.Goal, "Kill enemies as fast as possible: Alklha is eating the Moon! If the Moon disappears, you lose!" },
            { TutorialAction.MoveInstructions, "Move with WASD!" },
            { TutorialAction.BasicAttackInstructions, "Attack using MOUSE LEFT!" },
            { TutorialAction.EnergyPowerUpCollected, "You picked up a healing crystal. It will provide you some useful energy!" },
            { TutorialAction.ElementalAttackCollected, "You picked up a fire crystal. Now you can use your tale as a weapon! Press E!" },
            { TutorialAction.MoonshotReadyInstructions, "Moonshot ready! Press Q to unleash!" },
            { TutorialAction.RestoreMoon, "Alklha is chasing you! Damage him as much as possible to restore part of the Moon!" },
            { TutorialAction.EnergyInstructions, "Don't get hit or you will lose your light! Collect Moon crystals to recover!" }
        };
        #endregion

        printed = new bool[tutorial.Count];
        for (int i = 0; i < printed.Length; i++)
        {
            printed[i] = false;
        }

        AddToOutputQueue(TutorialAction.Goal);
        AddToOutputQueue(TutorialAction.MoveInstructions);
        AddToOutputQueue(TutorialAction.BasicAttackInstructions);
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

    public void AddToOutputQueue(TutorialAction action)
    {
        if (printed[(int)action])
            return;

        printed[(int)action] = true;
        outputInstructionsQueue.Enqueue(action);
        if (instructionsCoroutine == null)
        {
            instructionsCoroutine = StartCoroutine(PrintTextInQueue());
        }
    }

    /** <summary> we are cool man !</summary>*/
    private IEnumerator PrintTextInQueue()
    {
        while (outputInstructionsQueue.Count > 0)
        {
            TutorialAction action = outputInstructionsQueue.Dequeue();
            yield return StartCoroutine(PrintOnScreen(tutorial[action]));
        }
        instructionsCoroutine = null;
    }

    public void EnableMoonshotUI()
    {
        moonshotText.enabled = true;
        moonshotImg.color = enabledColor;
        AddToOutputQueue(TutorialAction.MoonshotReadyInstructions);
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

    IEnumerator PrintOnScreen(string s)
    {
        Debug.Log("Print on screen: " + s);
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

    public void OnGameOver(bool hasWon)
    {
        if (!gameEnded)
        {
            gameEnded = true;
            StartCoroutine(FadeGameOver(hasWon));
        }
    }

    private IEnumerator FadeGameOver(bool hasWon)
    {
        yield return new WaitForSeconds(5.0f);
        pauseMenu.enabled = !pauseMenu.enabled;
        pauseTitle.SetActive(false);
        resumeGameButton.gameObject.SetActive(false);
        if (hasWon)
        {
            endGameText.text = "You saved the Moon and banished Darkness from our World!\n" +
                + Mathf.Round(GameManager.Instance.Moon.Integrity) + " %" +
                "\nWill you be a better saviour next time?";
        }
        else
        {
            endGameText.text = "The moon is lost and the World has fallen into Darkness...\n" +
                "You will need to do better next time...";
        }
    }
}
