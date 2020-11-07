using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform enemy;
    
    // TODO: Kamera soll immer Spieler und Gegner sehen
    void Start()
    {
        this.transform.position = new Vector3(this.transform.position.x, -11, this.transform.position.z);
        this.StartCoroutine(this.CIntro());
    }

    private IEnumerator CIntro()
    {
        yield return new WaitForSeconds(0.5f);
        
        var targetPos = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPos, "time", 2, "easeType", "easeInOutCubic"));
    }

    void Update()
    {
        /*var pos = this.transform.position;
        pos.y = this.player.position.y;
        this.transform.position = pos;*/
    }
}
