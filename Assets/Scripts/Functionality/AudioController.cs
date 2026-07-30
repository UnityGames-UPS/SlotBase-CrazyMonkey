using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource bg_adudio;
    [SerializeField] internal AudioSource audioPlayer_wl;
    [SerializeField] internal AudioSource audioPlayer_button;
    [SerializeField] internal AudioSource audioSpin_button;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioClip[] Bonusclips;
    [SerializeField] private AudioSource bg_audioBonus;
    [SerializeField] private AudioSource audioPlayer_Bonus;

    private bool isForceMuted = false;
    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();

    private void Start()
    {
        if (bg_adudio) bg_adudio.Play();
        audioPlayer_button.clip = clips[clips.Length-1];
        audioSpin_button.clip = clips[clips.Length-2];
    }

    // Focus-driven mute. Called from BOTH the WebGL/JS OnFocusChanged path (UIManager)
    // and Unity's native OnApplicationFocus (SlotBehaviour) - both must share this one method.
    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;   // already in that state - don't re-capture/re-restore
        isForceMuted = forceMute;

        foreach (AudioSource source in AllSources())
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }

    private AudioSource[] AllSources()
    {
        return new AudioSource[] { bg_adudio, audioPlayer_wl, audioPlayer_button, audioSpin_button, bg_audioBonus, audioPlayer_Bonus };
    }

    // Every user-driven mute write goes through here so an in-effect focus mute
    // can't restore over a setting the player changed in the meantime.
    private void ApplyUserMute(AudioSource source, bool mute)
    {
        if (source == null) return;
        source.mute = mute;
        if (isForceMuted) preFocusMuteState[source] = mute;
    }

    internal void SwitchBGSound(bool isbonus)
    {
        if(isbonus)
        {
            if (bg_audioBonus) bg_audioBonus.enabled = true;
            if (bg_adudio) bg_adudio.enabled = false;
        }
        else
        {
            if (bg_audioBonus) bg_audioBonus.enabled = false;
            if (bg_adudio) bg_adudio.enabled = true;
        }
    }

    internal void PlayWLAudio(string type)
    {
        audioPlayer_wl.loop = false;
        int index = 0;
        switch (type)
        {
            case "spin":
                index = 0;
                audioPlayer_wl.loop = true;
                break;
            case "win":
                index = 1;
                break;
            case "lose":
                index = 2;
                break;
            case "spinStop":
                index = 3;
                break;
        }
        StopWLAaudio();
        audioPlayer_wl.clip = clips[index];
        audioPlayer_wl.Play();

    }

    internal void PlayBonusAudio(string type)
    {
        audioPlayer_wl.loop = false;
        int index = 0;
        switch (type)
        {
            case "coconut":
                index = 0;
                break;
            case "card":
                index = 1;
                break;
            case "lose":
                index = 2;
                break;
            case "win":
                index = 3;
                break;
        }
        StopBonusAaudio();
        audioPlayer_Bonus.clip = Bonusclips[index];
        audioPlayer_Bonus.Play();

    }

    internal void PlayButtonAudio()
    {
        audioPlayer_button.Play();
    }

    internal void PlaySpinButtonAudio()
    {
        audioSpin_button.Play();
    }

    internal void StopWLAaudio()
    {
        audioPlayer_wl.Stop();
        audioPlayer_wl.loop = false;
    }

    internal void StopBonusAaudio()
    {
        audioPlayer_Bonus.Stop();
        audioPlayer_Bonus.loop = false;
    }

    internal void StopBgAudio()
    {
        bg_adudio.Stop();
    }

    internal void ToggleMute(bool toggle, string type="all")
    {
        switch (type)
        {
            case "bg":
                ApplyUserMute(bg_adudio, toggle);
                break;
            case "button":
                ApplyUserMute(audioPlayer_button, toggle);
                ApplyUserMute(audioSpin_button, toggle);
                break;
            case "wl":
                ApplyUserMute(audioPlayer_wl, toggle);
                break;
            case "all":
                ApplyUserMute(audioPlayer_wl, toggle);
                ApplyUserMute(bg_adudio, toggle);
                ApplyUserMute(audioPlayer_button, toggle);
                ApplyUserMute(audioSpin_button, toggle);
                break;
        }
    }

    internal void   ChangeVolume(string type, float vol)
    {
        switch (type)
        {
            case "bg":

                ApplyUserMute(bg_adudio, vol == 0);
                bg_adudio.volume = vol;
                ApplyUserMute(bg_audioBonus, vol == 0);
                bg_audioBonus.volume = vol;
                break;
            case "button":
                ApplyUserMute(audioPlayer_button, vol == 0);
                audioPlayer_button.volume = vol;
                break;
            case "wl":
                ApplyUserMute(audioPlayer_wl, vol == 0);
                audioPlayer_wl.volume = vol;
                ApplyUserMute(audioPlayer_Bonus, vol == 0);
                audioPlayer_Bonus.volume = vol;
                audioSpin_button.volume = vol;
                break;
            case "all":

                ApplyUserMute(audioPlayer_wl, vol == 0);
                ApplyUserMute(bg_adudio, vol == 0);
                ApplyUserMute(audioPlayer_button, vol == 0);
                audioPlayer_wl.volume = vol;
                bg_adudio.volume = vol;
                audioPlayer_button.volume = vol;
                break;
        }

    }

}
