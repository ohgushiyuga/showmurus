using UnityEngine;
using System.Collections;

public class DamageFlasher : MonoBehaviour
{
    public Color damageColor = Color.red; //ダメージを受けたときに光らせる色
    public float flashDuration = 0.2f; //点滅する時間

    public string baseColorPropertyName = "_BaseColor";
    public string emissionColorPropertyName = "_EmissionColor";

    // --- 内部変数 ---
    // 1つの Renderer ではなく、Renderer の「配列」にする
    private Renderer[] bossRenderers; 
    
    // それぞれのRendererの元の色を保存する配列
    private Color[] originalBaseColors;
    private Color[] originalEmissionColors;
    
    private MaterialPropertyBlock propBlock; //見られたときに使える人とみられるから理解しておいたほうがいい
    private int baseColorId;
    private int emissionColorId;

    private Coroutine flashCoroutine;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        baseColorId = Shader.PropertyToID(baseColorPropertyName);
        emissionColorId = Shader.PropertyToID(emissionColorPropertyName);

        // 子オブジェクトから「すべて」の Renderer を取得（非アクティブ含む）
        bossRenderers = GetComponentsInChildren<Renderer>(true);

        if (bossRenderers.Length == 0)
        {
            Debug.LogError("子オブジェクトに Renderer が1つも見つかりません。", this.gameObject);
            return;
        }
        
        Debug.Log($"合計 {bossRenderers.Length} 個の Renderer を取得しました。");

        // すべての Renderer の元の色を保存
        originalBaseColors = new Color[bossRenderers.Length];
        originalEmissionColors = new Color[bossRenderers.Length];

        for (int i = 0; i < bossRenderers.Length; i++)
        {
            Material sharedMat = bossRenderers[i].sharedMaterial;
            
            if (sharedMat.HasProperty(baseColorId))
                originalBaseColors[i] = sharedMat.GetColor(baseColorId);
            else
                originalBaseColors[i] = Color.white;

            if (sharedMat.HasProperty(emissionColorId))
                originalEmissionColors[i] = sharedMat.GetColor(emissionColorId);
            else
                originalEmissionColors[i] = Color.black;
        }
    }

    public void Flash()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (bossRenderers == null || bossRenderers.Length == 0) return;
        Debug.Log("Flash() が実行されました！ (MPB / 複数)");

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            ResetColor(); // 念のためリセット
        }

        flashCoroutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        // すべての Renderer をループで赤くする
        foreach (Renderer rend in bossRenderers)
        {
            rend.GetPropertyBlock(propBlock);
            
            // _BaseColor をダメージ色に
            propBlock.SetColor(baseColorId, damageColor);
            
            // 発光もダメージ色に
            propBlock.SetColor(emissionColorId, damageColor * 2.0f); // 2倍の強さ

            rend.SetPropertyBlock(propBlock);
        }
        
        yield return new WaitForSeconds(flashDuration);
        
        ResetColor();
        flashCoroutine = null;
    }

    // 色を元の状態に戻す
    private void ResetColor()
    {
        if (bossRenderers == null) return;
        
        // すべての Renderer をループで元の色に戻す
        for(int i = 0; i < bossRenderers.Length; i++)
        {
            Renderer rend = bossRenderers[i];
            
            rend.GetPropertyBlock(propBlock);
            
            // _BaseColor を元の色に
            propBlock.SetColor(baseColorId, originalBaseColors[i]);
            
            // 発光を元の色に
            propBlock.SetColor(emissionColorId, originalEmissionColors[i]);
            
            rend.SetPropertyBlock(propBlock);
        }
    }
}