using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Zenject;

namespace Unbeetleble.Game
{
    public class PoisonBullet : MonoBehaviour
    {
        [Inject]
        private MusicController musicController;
        
        [Inject]
        private Player player;

        [SerializeField]
        private new Rigidbody2D rigidbody;
        
        [SerializeField]
        private new ParticleSystem particleSystem;

        [SerializeField]
        private float speed;

        private ParticleSystem.Particle[] particles;

        private float checkTimer = 0;
        
        void Start()
        {
            this.particles = new ParticleSystem.Particle[this.particleSystem.main.maxParticles];
        }
        
        void Update()
        {
            if (!this.particleSystem.isEmitting)
            {
                this.Invoke(() => { Object.Destroy(this); }, 3f);
            }

            this.checkTimer += Time.deltaTime;
            if (this.checkTimer >= 0.1f)
            {
                this.checkTimer = 0;
                this.particleSystem.GetParticles(this.particles);

                foreach (ParticleSystem.Particle particle in this.particles)
                {
                    if (particle.GetCurrentColor(this.particleSystem).a > 100)
                    {
                        float distance = Vector2.Distance(new Vector2(this.transform.position.x + particle.position.x, this.transform.position.y + particle.position.y), new Vector2(this.player.transform.position.x, this.player.transform.position.y));
                        if (distance < 0.3f)
                        {
                            this.player.OnPoisonCollision();
                            break;
                        }
                    }
                }
            }
        }

        public void ShootTo(Vector3 targetPosition)
        {
            this.rigidbody.velocity = (targetPosition - this.transform.position).normalized * this.speed;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.transform.position.y < this.transform.position.y && this.rigidbody.velocity.y < 1)
            {
                this.rigidbody.bodyType = RigidbodyType2D.Static;
                this.rigidbody.isKinematic = true;
            }
        }
    }
}