using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButtonSound : MonoBehaviour
{
    public AudioClip buttonPressed;
    public AudioClip buttonHovered;

    AudioSource audio;
    
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayHoverAudio()
    {
        audio.PlayOneShot(buttonHovered);
    }

    public void PlayPressedAudio()
    {
        audio.PlayOneShot(buttonPressed);
    }
}
