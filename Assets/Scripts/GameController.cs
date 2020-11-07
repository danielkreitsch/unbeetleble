using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Volume volume;

    private UnityEngine.Rendering.Universal.ChromaticAberration chromaticAberration;

    public void ScreenEffect1()
    {
        this.StartCoroutine(this.CScreenEffect1());
    }

    private IEnumerator CScreenEffect1()
    {
        if (this.volume.profile.TryGet(out this.chromaticAberration))
        {
            for (float a = 0; a < 1; a += 8 * Time.deltaTime)
            {
                this.chromaticAberration.intensity.Override(0.5f * a);
                yield return new WaitForEndOfFrame();
            }
            for (float a = 1; a > 0; a -= 8 * Time.deltaTime)
            {
                this.chromaticAberration.intensity.Override(0.5f * a);
                yield return new WaitForEndOfFrame();
            }
            this.chromaticAberration.intensity.Override(0);
        }
    }
}
