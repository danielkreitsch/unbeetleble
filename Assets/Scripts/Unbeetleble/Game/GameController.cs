using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Unbeetleble.Game
{
    public class GameController : MonoBehaviour
    {
        [Inject]
        private new Camera camera;

        [Inject]
        private Player player;

        [Inject]
        private Enemy enemy;

        [SerializeField]
        private Volume volume;

        // Effects
        private UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;
        private UnityEngine.Rendering.Universal.Vignette vignette;

        public void ScreenEffect1()
        {
            this.StartCoroutine(this.CScreenEffect1());
        }

        public void ScreenShake()
        {
            this.StartCoroutine(this.CScreenShake());
        }

        public void SetVignette(Color color)
        {
            if (this.volume.profile.TryGet(out this.vignette))
            {
                this.vignette.color.Override(this.vignette.color.value + (color - this.vignette.color.value) * 5 * Time.deltaTime);
            }
        }

        private IEnumerator CScreenEffect1()
        {
            if (this.volume.profile.TryGet(out this.chromaticAberration))
            {
                for (float a = 0; a < 1; a += 8 * Time.deltaTime)
                {
                    this.chromaticAberration.intensity.Override(1 * a);
                    yield return new WaitForEndOfFrame();
                }
                for (float a = 1; a > 0; a -= 8 * Time.deltaTime)
                {
                    this.chromaticAberration.intensity.Override(1 * a);
                    yield return new WaitForEndOfFrame();
                }
                this.chromaticAberration.intensity.Override(0);
            }
        }

        private IEnumerator CScreenShake()
        {
            if (this.volume.profile.TryGet(out this.chromaticAberration))
            {
                for (int i = 1; i <= 3; i++)
                {
                    var angles = this.camera.transform.localEulerAngles;
                    angles.z = Random.Range(-0.4f * i, 0.4f * i);
                    this.camera.transform.localEulerAngles = angles;

                    this.chromaticAberration.intensity.Override(0.2f * i);

                    yield return new WaitForSeconds(0.03f);
                }

                for (int i = 3; i >= 0; i--)
                {
                    var angles = this.camera.transform.localEulerAngles;
                    angles.z = Random.Range(-0.4f * i, 0.4f * i);
                    this.camera.transform.localEulerAngles = angles;

                    this.chromaticAberration.intensity.Override(0.2f * i);

                    yield return new WaitForSeconds(0.03f);
                }

                {
                    var angles = this.camera.transform.localEulerAngles;
                    angles.z = 0;
                    this.camera.transform.localEulerAngles = angles;
                }
            }
        }

        void OnGUI()
        {
            GUI.Box(new Rect(20, 20, 90, 40), "Player: " + this.player.health + "\nEnemy: " + this.enemy.Health);
        }
    }
}