using System.Collections;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private Border border;

    [SerializeField]
    private LivingEntity target;

    [SerializeField]
    private GameObject digParticlesPrefab;

    [SerializeField]
    private Transform model;

    [Header("Wayfinding")]
    [SerializeField]
    private Transform raycastOrigin;

    [Header("Jumping")]
    [SerializeField]
    private float jumpSpeed;

    [Header("Attacking")]
    [SerializeField]
    private float attackRange;

    [SerializeField]
    private int maxAttackAttempts;

    private State state;

    private DigParticles digParticles;

    private int attackCounter = 0;

    //private float timer = 0;

    void Start()
    {
        this.SetState(State.DigOut);
    }

    void Update()
    {
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
        else if (state == State.Attack1)
        {
            this.StartCoroutine(this.State_Attack1());
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
        this.attackCounter = 0;

        this.model.gameObject.SetActive(false);

        this.TeleportToHoleEntry(this.GetRandomHoleEntry());
        yield return new WaitForSeconds(2);

        // Next state
        this.SetState(State.DigOut);
    }

    IEnumerator State_DigOut()
    {
        var holeEntry = this.GetRandomHoleEntry();
        this.TeleportToHoleEntry(holeEntry);

        this.digParticles = this.SpawnDigParticles(holeEntry.position, 0.1f);
        yield return new WaitForSeconds(2);
        this.digParticles.SetIntensity(0.5f);
        yield return new WaitForSeconds(2);
        this.digParticles.SetIntensity(1);

        this.digParticles.Remove();

        this.model.gameObject.SetActive(true);

        // Next state
        if (Random.Range(0, 1) == 0)
        {
            this.SetState(State.Attack1);
        }
        else
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                this.model.transform.localPosition = new Vector3(0, -1.15f + i * (1.15f - 0.65f), 0);
                yield return new WaitForEndOfFrame();
            }
            this.model.transform.localPosition = new Vector3(0, -0.65f, 0);
            this.SetState(State.IdleOutside);
        }
    }

    IEnumerator State_IdleOutside()
    {
        yield return new WaitForSeconds(2);

        // Next state
        if (Random.Range(0, 1) == 0)
        {
            this.SetState(State.Attack1);
        }
        else
        {
            this.SetState(State.DigIn);
        }
    }

    IEnumerator State_Attack1()
    {
        HoleEntry holeEntry = this.GetHoleEntryBehindTarget();
        Vector3 holeEntryPosition = new Vector3(holeEntry.position.x, holeEntry.position.y, 0);
        Vector3 targetPosition = holeEntryPosition + (holeEntryPosition - this.transform.position).normalized * 1;
        float distanceToTarget = Vector2.Distance(this.transform.position, targetPosition);

        this.transform.LookAt(targetPosition);

        float jumpTime = distanceToTarget / this.jumpSpeed;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPosition, "time", jumpTime, "easeType", "linear"));

        bool attacked = false;

        for (float time = 0; time < jumpTime; time += Time.deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(this.transform.position, this.target.transform.position);

            if (!attacked && distanceToPlayer < this.attackRange)
            {
                attacked = true;
                this.target.ReceiveDamage(3);
                this.gameController.ScreenShake();
            }

            yield return new WaitForEndOfFrame();
        }

        /*for (float timeout = 0; timeout < 5; timeout += Time.deltaTime) // Cancel loop after 5 seconds
        {
            float distanceToTarget = Vector2.Distance(this.transform.position, targetPosition);
            float distanceToPlayer = Vector2.Distance(this.transform.position, this.target.transform.position);
            float progress = 1 - distanceToTarget / startDistanceToTarget;

            if (!attacked && distanceToPlayer < 1.5f)
            {
                attacked = true;
                this.target.ReceiveDamage(3);
                this.gameController.ScreenShake();
            }

            if (distanceToTarget < 0.05f)
            {
                break;
            }

           // iTween.MoveTo();
            if (progress < 0.9f)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * 2);
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * 5);
            }

            yield return new WaitForEndOfFrame();
        }*/

        this.TeleportToHoleEntry(holeEntry);

        yield return new WaitForSeconds(0.5f);

        this.attackCounter += 1;

        // Next state
        if (this.attackCounter < this.maxAttackAttempts && !attacked)
        {
            this.SetState(State.Attack1);
        }
        else
        {
            this.SetState(State.InEarth);
        }
    }

    private DigParticles SpawnDigParticles(Vector3 position, float intensity)
    {
        GameObject digParticlesObj = Object.Instantiate(this.digParticlesPrefab, position, Quaternion.identity);
        DigParticles digParticles = digParticlesObj.GetComponent<DigParticles>();
        digParticles.SetIntensity(intensity);
        return digParticles;
    }

    private HoleEntry GetHoleEntryBehindTarget()
    {
        var hit = Physics2D.Raycast(this.raycastOrigin.position, this.target.transform.position - this.transform.position, 1000, this.border.layer);

        if (hit.collider == null)
        {
            Debug.LogError("Border behind player not found.");
            return null;
        }

        return this.border.GetClosestHoleEntry(hit.point);
    }

    private HoleEntry GetRandomHoleEntry()
    {
        return this.border.holeEntries[Random.Range(0, this.border.holeEntries.Count)];
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
        Attack1,
        Jump
    }
}