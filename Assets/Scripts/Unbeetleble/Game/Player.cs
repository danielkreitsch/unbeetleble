using System;
using UnityEngine;
using Zenject;

namespace Unbeetleble.Game
{
    public class Player : MonoBehaviour
    {
        [Inject]
        private GameController gameController;

        public float health;

        private bool poisonTrigger = false;
        private float poisonCheckTimer = 0;
        private float poisonTimeout = 0;

        public void OnDamageReceive(float damage)
        {
            this.health = Math.Max(0, this.health - damage);
        }

        void Start()
        {
        }

        void Update()
        {
            this.poisonCheckTimer += Time.deltaTime;
            if (this.poisonCheckTimer >= 0.1f)
            {
                this.poisonCheckTimer = 0;
                if (this.poisonTrigger)
                {
                    this.poisonTimeout = 0.3f;
                    this.poisonTrigger = false;
                    this.OnDamageReceive(0.1f);
                }
            }

            if (this.poisonTimeout > 0)
            {
                this.poisonTimeout -= Time.deltaTime;
                this.gameController.SetVignette(new Color(0, 0.5f, 0));
            }
            else
            {
                this.gameController.SetVignette(new Color(0, 0, 0));
            }
        }

        public void OnPoisonCollision()
        {
            this.poisonTrigger = true;
        }
    }
}