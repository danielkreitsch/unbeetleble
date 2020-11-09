using System.Collections;
using UnityEngine;
using Zenject;

namespace Unbeetleble.Game
{
    public class CameraController : MonoBehaviour
    {
        [Inject]
        private new Camera camera;
        
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
        private bool gameEnded = false;

        private Vector3 defaultPosition;

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

        public void OnWin()
        {
            this.StartCoroutine(this.COnWin());
        }

        private IEnumerator COnWin()
        {
            this.gameEnded = true;

            var targetPos = this.startPosition;
            yield return new WaitForSeconds(2.5f);
            iTween.MoveTo(this.gameObject, iTween.Hash("position", targetPos, "time", 2, "easeType", "easeInOutCubic"));
        }

        void Update()
        {
            if (this.introDone && !this.gameEnded)
            {
                var mousePos = this.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -this.camera.transform.position.z));
                mousePos.z = 0;

                this.transform.position = Vector3.Lerp(this.transform.position,
                    this.defaultPosition + new Vector3(mousePos.x * this.movementToMouse, mousePos.y * this.movementToMouse, 0),
                    5 * Time.deltaTime);
            }
        }
    }
}