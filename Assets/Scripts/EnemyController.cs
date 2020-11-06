using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Border border;
    
    private State state = State.Start;

    private float timer = 0;
    
    void Start()
    {
        //this.TeleportToRandomHole();
    }

    void Update()
    {
        this.timer += Time.deltaTime;

        if (this.timer > 1)
        {
            this.timer = 0;
            this.TeleportToRandomHole();
        }
    }

    private void TeleportToRandomHole()
    {
        this.transform.position = this.border.holeEntries[Random.Range(0, this.border.holeEntries.Count)];
    }

    enum State
    {
        Start,
        Idle,
        DigIn,
        DigOut,
        JumpThroughPlayer
    }
}