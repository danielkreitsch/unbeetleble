using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource standardAudio;
    
    [SerializeField]
    private AudioSource battleAudio;

    public void SetVolume(float standard, float battle)
    {
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
}
