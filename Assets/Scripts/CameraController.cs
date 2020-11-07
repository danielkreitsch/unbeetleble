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

    [SerializeField]
    private float movementToMouse;
    
    [SerializeField]
    private bool intro;

    [SerializeField]
    private Vector3 startPosition;

    private bool introDone = false;

    private Vector3 defaultPosition;

    // TODO: Kamera soll immer Spieler und Gegner sehen
    void Start()
    {
        this.defaultPosition = this.transform.position;
        
        if (this.intro)
        {
            this.transform.position = this.startPosition;
            this.StartCoroutine(this.CIntro());
        }
    }

    private IEnumerator CIntro()
    {
        var targetPos = this.defaultPosition;
        yield return new WaitForSeconds(0.5f);
        iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPos, "time", 2, "easeType", "easeInOutCubic"));
        yield return new WaitForSeconds(2);
        this.introDone = true;
    }
    
    void Update()
    {
        if (this.introDone)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            mousePos.z = 0;
            
            this.transform.position = Vector3.Lerp(this.transform.position,
                this.defaultPosition + new Vector3(mousePos.x * this.movementToMouse, mousePos.y * this.movementToMouse, 0),
                5 * Time.deltaTime);
        }
    }
}