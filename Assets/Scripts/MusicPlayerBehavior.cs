using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayerBehavior : MonoBehaviour
{
    public string synthChannelName = "synth0Vol";
    public bool isPlayingSynth = false;
    public AudioMixer masterMixer;
    void Start()
    {
        if(!isPlayingSynth) {
            masterMixer.SetFloat(synthChannelName, -80.0f);
        }
    }
}
