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
}
