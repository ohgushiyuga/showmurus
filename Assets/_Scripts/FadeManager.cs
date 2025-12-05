using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameSceneFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private float waitTime = 2.0f;

    private void Start()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(waitTime);

        seq.Append(canvasGroup.DOFade(0f, fadeTime));

        seq.OnComplete(() => 
        {
            canvasGroup.blocksRaycasts = false;
        });
    }
}