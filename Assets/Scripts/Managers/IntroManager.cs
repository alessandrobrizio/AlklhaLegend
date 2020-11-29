using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    #region References
    [Header("Intro Menu Objs")]
    public TextMeshProUGUI textIntro;

    [Header("Button Sounds")]
    public AudioClip hoverSound;
    public AudioClip pressSound;

    [Header("Time Settings")]
    [Tooltip("Time passed before intro is shown up")]
    [SerializeField] float introDelay;
    [SerializeField] float fadingTime;
    [Tooltip("The time the single text is visible")]
    [SerializeField] float textTTL;
    #endregion

    //Local Vars
    int counter;
    string[] intros;
    AudioSource aSource;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();

        intros = new string[7];
        intros[0] = "Based on a siberian legend...";
        intros[1] = "Why is the Moon scarred?";
        intros[2] = "They’re fang marks left by Alklha, a demon whose aim is to bring an eternal night over the Earth";
        intros[3] = "Alklha is the personification of Darkness. It feeds on the Moon every month, slowly nibbling at it until it disappears.";
        intros[4] = "But the nature's protector does not agree with the monster.";
        intros[5] = "He fights the monster, making him spit the Moon piece by piece...eventually re-creating the full Moon.";
        intros[6] = "Will he save the Earth from Darkness?";

        counter = 0;

        StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        Color originalColor = textIntro.color;

        yield return new WaitForSeconds(introDelay);

        while (counter < intros.Length)
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

        yield return new WaitForSeconds(introDelay);

        SceneManager.LoadScene("MenuScene");
    }

    public void OnButtonHovered()
    {
        aSource.PlayOneShot(hoverSound);
    }

    public void OnButtonPressed()
    {
        aSource.PlayOneShot(pressSound);
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
