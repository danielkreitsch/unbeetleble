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

    void Update()
    {
        /*var pos = this.transform.position;
        pos.y = this.player.position.y;
        this.transform.position = pos;*/
    }
}
