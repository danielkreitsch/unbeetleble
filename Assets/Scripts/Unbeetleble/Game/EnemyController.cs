using System.Collections;
using UnityEngine;
using Utility;

namespace Unbeetleble.Game
{
    public class EnemyController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameController gameController;

        [SerializeField]
        private MusicController musicController;

        [SerializeField]
        private CameraController cameraController;

        [SerializeField]
        private Enemy enemy;

        [SerializeField]
        private Rigidbody2D rb;

        [SerializeField]
        private Collider hitboxCollider;

        [SerializeField]
        private Border border;

        [SerializeField]
        private LivingEntity target;

        [SerializeField]
        private GameObject digParticlesPrefab;

        [SerializeField]
        private GameObject poisonBulletPrefab;

        [SerializeField]
        private Transform model;

        [Header("Wayfinding")]
        [SerializeField]
        private Transform raycastOrigin;

        [Header("Jumping")]
        [SerializeField]
        private float jumpSpeed;

        [SerializeField]
        private string jumpMoveEaseType;

        [SerializeField]
        private string jumpRotateStartEaseType;

        [SerializeField]
        private string jumpRotateEndEaseType;

        [Header("Attacking")]
        [SerializeField]
        private float attackRange;

        [SerializeField]
        private int maxAttackAttempts;

        public float startTime;

        public State state;
        private State previousState;

        private DigParticles digParticles;

        private int attackCounter = 0;

        private bool forcePoisonAttack = false;

        private bool died = false;

        //private float timer = 0;

        void Start()
        {
            this.Invoke(() => this.SetState(State.InEarth), this.startTime);
        }

        void Update()
        {
            if (this.enemy.Health <= 0)
            {
                if (!this.died)
                {
                    this.died = true;
                    this.StartCoroutine(this.CDeath());
                }

                return;
            }
        }

        private void SetState(State state)
        {
            this.previousState = this.state;
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
            else if (state == State.Attack2)
            {
                this.StartCoroutine(this.State_Attack2());
            }
        }

        IEnumerator State_DigIn()
        {
            this.musicController.FadeToVolume(0.8f, 0.2f, 2);

            for (float time = 0; time < 1; time += 4 * Time.deltaTime)
            {
                this.model.transform.localPosition = new Vector3(0, -1.1f - time, 0);
                yield return new WaitForEndOfFrame();
            }
            this.model.transform.localPosition = new Vector3(0, -2.1f, 0);

            this.SetState(State.InEarth);
        }

        IEnumerator State_InEarth()
        {
            this.attackCounter = 0;

            this.hitboxCollider.enabled = false;
            this.model.gameObject.SetActive(false);

            this.transform.position = this.GetPositionInHoleEntry(this.GetRandomHoleEntry());
            yield return new WaitForSeconds(4);

            // Next state
            this.SetState(State.DigOut);
        }

        IEnumerator State_DigOut()
        {
            var holeEntry = this.GetRandomHoleEntry();
            this.TeleportToHoleEntry(holeEntry);

            this.digParticles = this.SpawnDigParticles(holeEntry.position, 0.1f);
            yield return new WaitForSeconds(2);
            this.digParticles.SetIntensity(0.3f);
            this.musicController.PlayDigOut();
            yield return new WaitForSeconds(2.5f);
            this.digParticles.SetIntensity(1);
            yield return new WaitForSeconds(2);

            this.digParticles.Remove();
            this.model.gameObject.SetActive(true);
            this.hitboxCollider.enabled = true;

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
            if (this.previousState == State.DigOut)
            {
                this.musicController.FadeToVolume(0.2f, 1, 1);
                this.musicController.PlayJumpOut();
            }

            HoleEntry holeEntry = this.GetHoleEntryBehindTarget();
            Vector3 targetPosition = this.GetPositionOnHoleEntry(holeEntry);
            Vector3 targetEulerAngles = this.GetEulerAnglesOnHoleEntry(holeEntry);

            float distanceToTarget = Vector2.Distance(this.transform.position, targetPosition);

            float jumpTime = distanceToTarget / this.jumpSpeed;
            iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPosition, "time", jumpTime, "easeType", this.jumpMoveEaseType));
            //iTween.RotateTo(this.gameObject, iTween.Hash("rotation", targetEulerAngles, "time", jumpTime, "easeType", this.jumpRotateEaseType));
            iTween.LookTo(this.gameObject, iTween.Hash("lookTarget", targetPosition, "time", 0.15f * jumpTime, "easeType", this.jumpRotateStartEaseType));
            //this.transform.eulerAngles = halfTargetEulerAngles;
            this.Invoke(() => iTween.RotateTo(this.gameObject, iTween.Hash("rotation", targetEulerAngles, "time", 0.15f * jumpTime, "easeType", this.jumpRotateEndEaseType)), 0.95f * jumpTime);

            yield return new WaitForSeconds(0.05f);

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

            this.transform.localEulerAngles = this.GetEulerAnglesOnHoleEntry(holeEntry);

            yield return new WaitForSeconds(1f);

            this.attackCounter += 1;

            // Next state
            if (this.attackCounter < this.maxAttackAttempts)
            {
                if (this.forcePoisonAttack)
                {
                    this.forcePoisonAttack = false;
                    if (Random.Range(0, 2) == 0)
                    {
                        this.SetState(State.Attack2);
                    }
                    else
                    {
                        this.SetState(State.Attack1);
                    }
                }
                else
                {
                    if (Random.Range(0, 4) == 0)
                    {
                        this.SetState(State.Attack2);
                    }
                    else
                    {
                        this.SetState(State.Attack1);
                    }
                }
            }
            else
            {
                this.SetState(State.DigIn);
            }
        }

        IEnumerator State_Attack2()
        {
            for (int y = 2; y >= 0; y--)
            {
                this.ShootPoisonBullet(this.target.transform.position + Vector3.up * y * 3 + new Vector3(0, 0, -1));
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.8f);

            this.attackCounter += 1;

            // Next state
            if (this.attackCounter < this.maxAttackAttempts)
            {
                this.SetState(State.Attack1);
            }
            else
            {
                this.SetState(State.DigIn);
            }
        }

        private IEnumerator CDeath()
        {
            this.rb.bodyType = RigidbodyType2D.Dynamic;
            this.rb.constraints = RigidbodyConstraints2D.None;
            this.rb.angularVelocity = 1;
            this.rb.drag = 0.5f;
            this.rb.velocity = 0.5f * this.rb.velocity;
            this.musicController.OnWin();
            this.cameraController.OnWin();
            yield return new WaitForSeconds(5f);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private PoisonBullet ShootPoisonBullet(Vector3 targetPosition)
        {
            GameObject bulletObj = Object.Instantiate(this.poisonBulletPrefab, this.transform.position, Quaternion.identity);
            PoisonBullet bullet = bulletObj.GetComponent<PoisonBullet>();
            bullet.ShootTo(targetPosition);
            return bullet;
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
            var hit = Physics2D.Raycast(this.raycastOrigin.position, this.target.transform.position - this.raycastOrigin.position, 1000, this.border.gameObject.layer);

            if (Vector2.Distance(hit.point, this.transform.position) < 10)
            {
                this.forcePoisonAttack = true;
                return this.border.GetFurthestHoleEntry(this.transform.position, this.target.transform.position);
            }

            if (hit.collider == null)
            {
                Debug.LogError("Border behind player not found.");
                return null;
            }

            return this.border.GetClosestHoleEntry(hit.point);
        }

        private HoleEntry GetRandomHoleEntry()
        {
            return this.border.HoleEntries[Random.Range(0, this.border.HoleEntries.Count)];
        }

        private void TeleportToHoleEntry(HoleEntry holeEntry)
        {
            this.transform.position = holeEntry.position;
            this.model.localPosition = new Vector3(0, -1.1f, 0);
        }

        private Vector3 GetPositionInHoleEntry(HoleEntry holeEntry)
        {
            return new Vector3(holeEntry.position.x, holeEntry.position.y, 0);
        }

        private Vector3 GetPositionOnHoleEntry(HoleEntry holeEntry)
        {
            if (holeEntry.facing == Border.Facing.Top)
            {
                return new Vector3(holeEntry.position.x, holeEntry.position.y + 1, 0);
            }
            if (holeEntry.facing == Border.Facing.Bottom)
            {
                return new Vector3(holeEntry.position.x, holeEntry.position.y - 1, 0);
            }
            if (holeEntry.facing == Border.Facing.Left)
            {
                return new Vector3(holeEntry.position.x - 1, holeEntry.position.y, 0);
            }
            if (holeEntry.facing == Border.Facing.Right)
            {
                return new Vector3(holeEntry.position.x + 1, holeEntry.position.y + 1, 0);
            }
            return Vector3.zero;
        }

        private Vector3 GetEulerAnglesOnHoleEntry(HoleEntry holeEntry)
        {
            if (holeEntry.facing == Border.Facing.Top)
            {
                return new Vector3(0, -180, 0);
            }
            if (holeEntry.facing == Border.Facing.Bottom)
            {
                return new Vector3(0, -180, 180);
            }
            if (holeEntry.facing == Border.Facing.Left)
            {
                return new Vector3(0, -180, -90);
            }
            if (holeEntry.facing == Border.Facing.Right)
            {
                return new Vector3(0, -180, 90);
            }
            return Vector3.zero;
        }

        public enum State
        {
            InEarth,
            IdleOutside,
            DigIn,
            DigOut,
            Attack1,
            Attack2
        }
    }
}