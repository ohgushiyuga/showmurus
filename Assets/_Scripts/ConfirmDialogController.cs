using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening.Plugins.Options;

public class ConfirmDialogOptions
{
    // ダイアログの種類に応じて読み込むプレハブのパス
    public string PrefabPath { get; set; }

    // 表示するテキスト
    public string Title { get; set; }
    public string Message { get; set; }

    // ボタンが押されたときに実行する処理
    public Action OnOK { get; set; }
    public Action OnCancel { get; set; }
}

public class ConfirmDialogController : MonoBehaviour
{
    // 現在アクティブなダイアログインスタンスを保持する静的プロパティ
    public static ConfirmDialogController ActiveDialog { get; private set; }

    [SerializeField] private Text titleText;

    private Action onOK;
    private Action onCancel;

    private void Awake()
    {
        // この新しいインスタンスをアクティブなダイアログとして設定
        ActiveDialog = this;
    }
    
    // 自身が破棄されるとき、静的参照を解除する
    private void OnDestroy()
    {
        if (ActiveDialog == this)
        {
            ActiveDialog = null;
        }
    }

    public void Initialize(ConfirmDialogOptions options)
    {
        if (titleText != null)
        {
            titleText.text = options.Title;
        }

        onOK = options.OnOK;
        onCancel = options.OnCancel;
    }
    
    public static void Show(ConfirmDialogOptions options)
    {
        if (ActiveDialog != null)
        {
            if (ActiveDialog.gameObject != null)
            {
                Destroy(ActiveDialog.gameObject);
            }
        }

        if (string.IsNullOrEmpty(options.PrefabPath))
        {
            Debug.LogError("PrefabPathが指定されていません！");
            return;
        }

        var prefab = Resources.Load<GameObject>(options.PrefabPath);
        if (prefab == null)
        {
            Debug.LogError(options.PrefabPath + " がResourcesフォルダに見つかりません。");
            return;
        }

        var obj = Instantiate(prefab);
        var controller = obj.GetComponent<ConfirmDialogController>();
        if (controller != null)
        {
            controller.Initialize(options);
        }
    }

    public void OnTapOK()
    {
        onOK?.Invoke();
        Destroy(gameObject);
    }

    public void OnTapCancel()
    {
        onCancel?.Invoke();
        Destroy(gameObject);
    }
}