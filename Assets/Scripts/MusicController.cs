﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource standardAudio;
    
    [SerializeField]
    private AudioSource battleAudio;

    private bool died = false;

    public void SetVolume(float standard, float battle)
    {
        if (this.died)
        {
            return;
        }
        
        this.standardAudio.volume = standard;
        this.battleAudio.volume = battle;
    }

    public void FadeToVolume(float standard, float battle, float time)
    {
        this.StartCoroutine(this.CFadeToVolume(standard, battle, time));
    }

    private IEnumerator CFadeToVolume(float standard, float battle, float time)
    {
        float startStandard = this.standardAudio.volume;
        float startBattle = this.battleAudio.volume;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            this.standardAudio.volume = startStandard + (standard - startStandard) * (t / time);
            this.battleAudio.volume = startBattle + (battle - startBattle) * (t / time);
            yield return new  WaitForEndOfFrame();
        }

        this.standardAudio.volume = standard;
        this.battleAudio.volume = battle;
    }

    public void OnDeath()
    {
        this.StartCoroutine(this.COnDeath());
    }

    private IEnumerator COnDeath()
    {
        this.died = true;
        for (float t = 0; t < 1; t += 0.5f * Time.deltaTime)
        {
            this.standardAudio.volume = Mathf.Min(this.standardAudio.volume, 1 - t);
            this.battleAudio.volume = Mathf.Max(this.battleAudio.volume, t);
            this.battleAudio.pitch = 1 - t;
            yield return new  WaitForEndOfFrame();
        }
    }
}