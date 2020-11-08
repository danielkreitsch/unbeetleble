﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource silence;

    [SerializeField]
    private AudioSource bassdrum;

    [SerializeField]
    private AudioSource flutes;

    [SerializeField]
    private AudioSource frenchHorns;

    [SerializeField]
    private AudioSource fullStringSet;

    [SerializeField]
    private AudioSource ghettoDrums;

    [SerializeField]
    private AudioSource maleChoir;

    [SerializeField]
    private AudioSource orchPercussion;

    private bool gameEnded = false;

    private float silenceVolume = 1;
    private float battleVolume = 0;

    public void SetVolume(float silence, float battle)
    {
        this.silenceVolume = silence;
        this.silence.volume = silence;

        this.battleVolume = battle;
        this.bassdrum.volume = battle;
        this.flutes.volume = battle;
        this.frenchHorns.volume = battle;
        this.fullStringSet.volume = battle;
        this.ghettoDrums.volume = battle;
        this.maleChoir.volume = battle;
        this.orchPercussion.volume = battle;
    }

    public void FadeToVolume(float silence, float battle, float time)
    {
        this.StartCoroutine(this.CFadeToVolume(silence, battle, time));
    }

    private IEnumerator CFadeToVolume(float silence, float battle, float time)
    {
        float startSilence = this.silenceVolume;
        float startBattle = this.battleVolume;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            this.SetVolume(startSilence + (silence - startSilence) * (t / time),
                startBattle + (battle - startBattle) * (t / time));
            yield return new WaitForEndOfFrame();
        }

        this.silence.volume = silence;
        this.bassdrum.volume = battle;
    }

    public void OnDeath()
    {
        this.StartCoroutine(this.COnDeath());
    }

    private IEnumerator COnDeath()
    {
        this.gameEnded = true;
        for (float t = 0; t < 1; t += 0.5f * Time.deltaTime)
        {
            this.SetVolume(Mathf.Min(this.silence.volume, 1 - t),
                Mathf.Max(this.bassdrum.volume, t));

            this.bassdrum.pitch = 1 - t;
            this.flutes.pitch = 1 - t;
            this.frenchHorns.pitch = 1 - t;
            this.fullStringSet.pitch = 1 - t;
            this.ghettoDrums.pitch = 1 - t;
            this.maleChoir.pitch = 1 - t;
            this.orchPercussion.pitch = 1 - t;

            yield return new WaitForEndOfFrame();
        }
    }

    public void OnWin()
    {
        this.StartCoroutine(this.COnWin());
    }

    private IEnumerator COnWin()
    {
        this.gameEnded = true;
        for (float t = 0; t < 1; t += 0.5f * Time.deltaTime)
        {
            this.SetVolume(Mathf.Max(this.silenceVolume, t),
                Mathf.Min(this.battleVolume, 1 - t));

            this.silence.pitch = 1 + 2 * t;
            yield return new WaitForEndOfFrame();
        }

        for (float t = 0; t < 1; t += 0.25f * Time.deltaTime)
        {
            this.SetVolume(1 - t, 0);
            yield return new WaitForEndOfFrame();
        }
    }
}