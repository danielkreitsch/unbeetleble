using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private Border border;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Transform model;

    [SerializeField]
    private Transform raycastOrigin;

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

        Debug.Log("State: " + state);

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
            this.model.transform.localPosition = new Vector3(0, -0.65f - i * (1.15f - 0.65f), 0);
            yield return new WaitForEndOfFrame();
        }
        this.model.transform.localPosition = new Vector3(0, -1.15f, 0);
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
            this.model.transform.localPosition = new Vector3(0, -1.15f + i * (1.15f - 0.65f), 0);
            yield return new WaitForEndOfFrame();
        }
        this.model.transform.localPosition = new Vector3(0, -0.65f, 0);

        // Next state
        if (Random.Range(0, 1) == 0)
        {
            this.SetState(State.Attack);
        }
        else
        {
            this.SetState(State.IdleOutside);
        }
    }

    IEnumerator State_IdleOutside()
    {
        yield return new WaitForSeconds(2);

        // Next state
        if (Random.Range(0, 1) == 0)
        {
            this.SetState(State.Attack);
        }
        else
        {
            this.SetState(State.DigIn);
        }
    }

    IEnumerator State_Attack()
    {
        var hit = Physics2D.Raycast(this.raycastOrigin.position, this.player.transform.position - this.transform.position, 1000, this.border.layer);

        if (hit.collider == null)
        {
            Debug.LogError("Border behind player not found.");
            yield break;
        }

        HoleEntry holeEntry = this.border.GetClosestHoleEntry(hit.point);
        Vector3 holeEntryPosition = new Vector3(holeEntry.position.x, holeEntry.position.y, 0);
        Vector3 targetPosition = holeEntryPosition + (holeEntryPosition - this.transform.position).normalized * 1;
        float startDistanceToTarget = Vector2.Distance(this.transform.position, targetPosition);
        bool attacked = false;

        for (float i = 0; i < 3; i += Time.deltaTime) //while (true)
        {
            float distanceToTarget = Vector2.Distance(this.transform.position, targetPosition);
            float distanceToPlayer = Vector2.Distance(this.transform.position, this.player.transform.position);
            float progress = 1 - distanceToTarget / startDistanceToTarget;

            if (!attacked && distanceToPlayer < 0.5f)
            {
                attacked = true;
                this.gameController.ScreenEffect1();
            }

            if (distanceToTarget < 0.05f)
            {
                break;
            }

            if (progress < 0.5f)
            {
                this.transform.position += (targetPosition - this.transform.position) * Time.deltaTime;
            }
            else
            {
                this.transform.position += (targetPosition - this.transform.position) * Time.deltaTime * 5;
            }

            yield return new WaitForEndOfFrame();
        }

        this.TeleportToHoleEntry(holeEntry);

        yield return new WaitForSeconds(0.5f);

        // Next state
        this.SetState(State.InEarth);
    }

    private void TeleportToRandomHoleEntry()
    {
        this.TeleportToHoleEntry(this.border.holeEntries[Random.Range(0, this.border.holeEntries.Count)]);
    }

    private void TeleportToHoleEntry(HoleEntry holeEntry)
    {
        this.transform.position = holeEntry.position;
        this.model.localPosition = new Vector3(0, -1.15f, 0);

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