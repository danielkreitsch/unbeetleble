using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform model;

    [SerializeField]
    private Border border;

    private State state;

    //private float timer = 0;

    void Start()
    {
        this.SetState(State.DigOut);
    }

    void Update()
    {
        /*if (this.timer > 0)
        {
            this.timer -= Time.deltaTime;
        }

        if (this.timer <= 0)
        {
            if (this.state == State.InEarth)
            {
                this.SetState(State.DigOut);
            }
            else if (this.state == State.IdleOutside)
            {
                this.SetState(State.DigIn);
            }
        }*/
    }

    private void SetState(State state)
    {
        this.state = state;

        if (state == State.DigIn)
        {
            this.StartCoroutine(this.State_DigIn());
        }
        else if (state == State.InEarth)
        {
            this.StartCoroutine(this.State_InEarth());
        }
        else if (state == State.DigOut)
        {
            this.StartCoroutine(this.State_DigOut());
        }
        else if (state == State.IdleOutside)
        {
            this.StartCoroutine(this.State_IdleOutside());
        }
        else if (state == State.Attack)
        {
            this.StartCoroutine(this.State_Attack());
        }
    }

    IEnumerator State_DigIn()
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            this.model.transform.localPosition = new Vector3(0, -i, 0);
            yield return new WaitForEndOfFrame();
        }
        this.model.transform.localPosition = new Vector3(0, -1, 0);
        this.SetState(State.InEarth);
    }
    
    IEnumerator State_InEarth()
    {
        yield return new WaitForSeconds(2);

        // Next state
        this.SetState(State.DigOut);
    }

    IEnumerator State_DigOut()
    {
        this.TeleportToRandomHoleEntry();
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            this.model.transform.localPosition = new Vector3(0, -1 + i, 0);
            yield return new WaitForEndOfFrame();
        }
        this.model.transform.localPosition = new Vector3(0, 0, 0);

        // Next state
        if (Random.Range(0, 2) == 0)
        {
            this.SetState(State.IdleOutside);
        }
        else
        {
            this.SetState(State.Attack);
        }
    }

    IEnumerator State_IdleOutside()
    {
        yield return new WaitForSeconds(2);

        // Next state
        if (Random.Range(0, 2) == 0)
        {
            this.SetState(State.DigIn);
        }
        else
        {
            this.SetState(State.Attack);
        }
    }
    
    IEnumerator State_Attack()
    {
        yield return new WaitForSeconds(1);

        // Next state
        if (Random.Range(0, 2) == 0)
        {
            this.SetState(State.DigIn);
        }
        else
        {
            this.SetState(State.IdleOutside);
        }
    }


    private void TeleportToRandomHoleEntry()
    {
        this.TeleportToHoleEntry(this.border.holeEntries[Random.Range(0, this.border.holeEntries.Count)]);
    }

    private void TeleportToHoleEntry(HoleEntry holeEntry)
    {
        this.transform.position = holeEntry.position;
        if (holeEntry.facing == Facing.Top)
        {
            this.transform.localEulerAngles = new Vector3(0, -180, 0);
        }
        else if (holeEntry.facing == Facing.Bottom)
        {
            this.transform.localEulerAngles = new Vector3(0, -180, 180);
        }
        else if (holeEntry.facing == Facing.Left)
        {
            this.transform.localEulerAngles = new Vector3(0, -180, -90);
        }
        else if (holeEntry.facing == Facing.Right)
        {
            this.transform.localEulerAngles = new Vector3(0, -180, 90);
        }
    }

    enum State
    {
        InEarth,
        IdleOutside,
        DigIn,
        DigOut,
        Attack,
        Jump
    }
}