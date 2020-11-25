using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    #region References
    [Header("Intro Menu Objs")]
    public TextMeshProUGUI textIntro;
    public GameObject introContainer;
    public GameObject mainMenuContainer;
    public GameObject creditsContainer;

    [Header("Time Settings")]
    [Tooltip("Time passed before intro is shown up")]
    [SerializeField] float introDelay;
    [Tooltip("Time passed before menu is shown up")]
    [SerializeField] float menuDelay;
    [SerializeField] float fadingTime;
    [Tooltip("The time the single text is visible")]
    [SerializeField] float textTTL;
    #endregion

    //Local Vars
    int counter;
    string[] intros;

    private void Start()
    {
        if (!textIntro)
            textIntro = GameObject.FindGameObjectWithTag("Legend").GetComponent<TextMeshProUGUI>();

        //Lo metto qui, perchè Unity mi fa bestemmiare tanto
        intros = new string[4];
        intros[0] = "Why is the moon scarred?";
        intros[1] = "They’re fang marks left by Alklha, a monster with huge, impenetrably black wings.";
        intros[2] = "Alklha is a personification of the darkness of the sky. It feeds on the moon every month, slowly nibbling at it until it disappears.";
        intros[3] = "But the moon does not agree with the monster, who vomits it out into the sky, bit by bit, eventually re-creating the full moon.";

        counter = 0;

        introContainer.SetActive(true);
        mainMenuContainer.SetActive(false);
        creditsContainer.SetActive(false);

        StartCoroutine(IntroManager());
    }

    IEnumerator IntroManager()
    {
        Color originalColor = textIntro.color;

        yield return new WaitForSeconds(introDelay);

        while(counter < intros.Length)
        {
            for (float t = 0.01f; t < fadingTime; t += Time.deltaTime)
            {
                textIntro.text = intros[counter];
                textIntro.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadingTime));
                yield return null;
            }

            yield return new WaitForSeconds(textTTL);

            for (float t = 0.01f; t < fadingTime; t += Time.deltaTime)
            {
                textIntro.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadingTime));
                yield return null;
            }

            counter++;
        }

        introContainer.SetActive(false);

        yield return new WaitForSeconds(introDelay);

        mainMenuContainer.SetActive(true);
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
