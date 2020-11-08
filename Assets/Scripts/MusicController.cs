using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource silence;
    
    [SerializeField]
    private AudioSource bassdrum;

    private bool gameEnded = false;

    public void SetVolume(float standard, float battle)
    {
        if (this.gameEnded)
        {
            return;
        }
        
        this.silence.volume = standard;
        this.bassdrum.volume = battle;
    }

    public void FadeToVolume(float standard, float battle, float time)
    {
        this.StartCoroutine(this.CFadeToVolume(standard, battle, time));
    }

    private IEnumerator CFadeToVolume(float standard, float battle, float time)
    {
        float startStandard = this.silence.volume;
        float startBattle = this.bassdrum.volume;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            this.silence.volume = startStandard + (standard - startStandard) * (t / time);
            this.bassdrum.volume = startBattle + (battle - startBattle) * (t / time);
            yield return new  WaitForEndOfFrame();
        }

        this.silence.volume = standard;
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
            this.silence.volume = Mathf.Min(this.silence.volume, 1 - t);
            this.bassdrum.volume = Mathf.Max(this.bassdrum.volume, t);
            this.bassdrum.pitch = 1 - t;
            yield return new  WaitForEndOfFrame();
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
            this.bassdrum.volume = Mathf.Min(this.silence.volume, 1 - t);
            this.silence.volume = Mathf.Max(this.bassdrum.volume, t);
            this.silence.pitch = 1 + 2 * t;
            yield return new WaitForEndOfFrame();
        }
    }
}
