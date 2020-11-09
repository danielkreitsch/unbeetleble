using UnityEngine;
using UnityEngine.Rendering;

namespace Unbeetleble.Game
{
    public class LaserAttack : MonoBehaviour
    {
        [SerializeField]
        private Transform body;

        [SerializeField]
        private new Collider collider;

        [SerializeField]
        private Volume volume;

        [SerializeField]
        private iTween.EaseType easeTypeStart;

        [SerializeField]
        private iTween.EaseType easeTypeEnd;

        public float bloomIntensity;

        private float laserWidth;

        public float LaserWidth
        {
            get => this.laserWidth;
        }

        private UnityEngine.Rendering.Universal.Bloom bloom;
        
        void Start()
        {
            this.SetLaserWidth(0.0001f);
        }

        public void Buildup(float laserBuildupTime)
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0001f, "to", 0.004f, "onupdate", "SetLaserWidth", "time", laserBuildupTime, "easeType", this.easeTypeStart));
        }

        public void Disappear(float time)
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.06f, "to", 0.0f, "onupdate", "SetLaserWidth", "time", time, "easeType", this.easeTypeEnd));
        }

        public void SetLaserWidth(float width)
        {
            this.laserWidth = width;

            if (width > 0.05f)
            {
                this.collider.GetComponent<Collider>().enabled = true;
            }
            else if (width < 0.05f)
            {
                this.collider.GetComponent<Collider>().enabled = false;
            }

            var scale = this.body.transform.localScale;
            scale.y = width;
            this.body.transform.localScale = scale;

            if (this.volume.profile.TryGet(out this.bloom))
            {
                this.bloom.intensity.Override(width * this.bloomIntensity);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}