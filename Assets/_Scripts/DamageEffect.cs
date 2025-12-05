using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] List<Image> DamageImgs = new List<Image>();
    [SerializeField] private float fadeSpeed = 2f;

    void Start()
    {
        foreach (Image img in DamageImgs)
        {
            if (img != null)
            {
                img.color = Color.clear;
            }
        }
    }
    
    void Update()
    {
        FadeImagesToClear();
    }

    public void PlayDamageEffect()
    {
        foreach (Image img in DamageImgs)
        {
            if (img != null)
            {
                img.color = new Color(0.7f, 0, 0, 0.7f);
            }
        }
    }

    // 透明に戻す関数
    void FadeImagesToClear()
    {
        foreach (Image img in DamageImgs)
        {
            if (img != null)
            {
                img.color = Color.Lerp(img.color, Color.clear, Time.deltaTime * fadeSpeed);
            }
        }
    }
}