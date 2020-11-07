using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    
    [SerializeField]
    private Volume volume;

    private UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;

    public void ScreenEffect1()
    {
        this.StartCoroutine(this.CScreenEffect1());
    }

    public void ScreenShake()
    {
        this.StartCoroutine(this.CScreenShake());
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
}
