using UnityEngine;
using System.Collections.Generic;

public class TranslucentMaterial : MonoBehaviour
{
    private Renderer[] renderers;
    
    // 元の色を保存するリスト（メッシュごとに色が違う可能性があるため）
    private List<Color> originalColors = new List<Color>();

    [SerializeField] private float transparentAlpha = 0.3f; // 透明度

    private bool isTransparent = false;

    void Awake()
    {
        // 自分自身と、すべての子オブジェクトのRendererをまとめて取得
        renderers = GetComponentsInChildren<Renderer>();

        // すべてのRendererの、元の色を保存しておく
        foreach (Renderer r in renderers)
        {
            // 各Rendererの現在の色をリストに追加
            originalColors.Add(r.material.color);
        }
    }

    // 透明にする
    public void ClearMaterialInvoke()
    {
        if (isTransparent) return;
        isTransparent = true;

        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                Color targetColor = r.material.color;
                targetColor.a = transparentAlpha;
                r.material.color = targetColor;
            }
        }
    }

    // 元に戻す
    public void NotClearMaterialInvoke()
    {
        if (!isTransparent) return;
        isTransparent = false;

        // 保存しておいた元の色に戻す
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }
}