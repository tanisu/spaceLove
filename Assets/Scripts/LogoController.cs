using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoController : MonoBehaviour
{
    [SerializeField] RectTransform rtf;
    void Start()
    {
        StartCoroutine(_logoView());
    }

    IEnumerator _logoView()
    {
        
        yield return StartCoroutine(_spaceLogo());
        SoundManager.I.PlaySE(SESoundData.SE.LOGO);
        float scale = 0;
        while(scale < 1)
        {
            scale += 0.025f;
            rtf.localScale = new Vector2(scale, scale);
            yield return new WaitForSeconds(0.001f);
        }
        SoundManager.I.PlaySE(SESoundData.SE.TITLE);
    }
    IEnumerator _spaceLogo()
    {
        SoundManager.I.PlaySE(SESoundData.SE.LOGO);
        float scale = 0;
        while (scale < 1)
        {
            scale += 0.025f;
            transform.localScale = new Vector2(scale, scale);
            yield return new WaitForSeconds(0.001f);
        }
    }
}
