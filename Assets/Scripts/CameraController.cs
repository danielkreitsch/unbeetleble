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

        var startPos = this.transform.position;
        var targetPos = new Vector3(this.transform.position.x, 0, this.transform.position.z);

        for (float i = 0; i < 1; i += 0.8f * Time.deltaTime)
        {
            this.transform.position = Vector3.Slerp(startPos, targetPos, i);
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = targetPos;
    }

    void Update()
    {
        /*var pos = this.transform.position;
        pos.y = this.player.position.y;
        this.transform.position = pos;*/
    }
}
