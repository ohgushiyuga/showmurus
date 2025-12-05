using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;
using System.Collections.Generic;

public class SensitivitySettings : MonoBehaviour
{
    [Header("UI設定")]
    public Slider sensitivitySlider;
    public TMP_InputField sensitivityInputField;

    [Header("Cinemachine設定")]
    public CinemachineInputAxisController[] targetControllers;

    [Header("デフォルト設定")]
    [SerializeField] private float defaultSensitivity = 1.0f;

    private const string SaveKey = "MouseSensitivity";
    
    // 反転設定を覚えておくための辞書
    private Dictionary<object, bool> invertSettings = new Dictionary<object, bool>();

    void Start()
    {
        // 保存値の読み込み
        float savedValue = PlayerPrefs.GetFloat(SaveKey, defaultSensitivity);

        // スライダーの初期化
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedValue;
            sensitivitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // Input Fieldの初期化
        if (sensitivityInputField != null)
        {
            sensitivityInputField.text = savedValue.ToString("F2");
            sensitivityInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }

        // 最初の適用（ここでカメラを探しに行きます）
        ApplySensitivity(savedValue);
    }

    // カメラを探して、反転設定を覚えるセットアップ関数
    private bool TryFindAndSetupControllers()
    {
        // 既にリストが入っているなら何もしない
        if (targetControllers != null && targetControllers.Length > 0 && targetControllers[0] != null)
        {
            return true;
        }

        // カメラを探す
        targetControllers = FindObjectsByType<CinemachineInputAxisController>(FindObjectsSortMode.None);

        if (targetControllers != null && targetControllers.Length > 0)
        {
            Debug.Log($"[Sensitivity] カメラコントローラーを {targetControllers.Length} 個発見しました。");
            
            // 見つかったタイミングで反転設定を記憶する
            foreach (var axisController in targetControllers)
            {
                if (axisController == null) continue;
                foreach (var controllerEntry in axisController.Controllers)
                {
                    bool isInverted = controllerEntry.Input.Gain < 0;
                    invertSettings[controllerEntry] = isInverted;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnClickDefault()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = defaultSensitivity;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        ApplySensitivity(value);
        PlayerPrefs.SetFloat(SaveKey, value);
        PlayerPrefs.Save();

        if (sensitivityInputField != null && sensitivityInputField.text != value.ToString("F2"))
        {
            sensitivityInputField.text = value.ToString("F2");
        }
    }

    public void OnInputFieldEndEdit(string text)
    {
        if (float.TryParse(text, out float value))
        {
            if (sensitivitySlider != null)
            {
                value = Mathf.Clamp(value, sensitivitySlider.minValue, sensitivitySlider.maxValue);
                sensitivitySlider.value = value;
            }
        }
        else
        {
            if (sensitivityInputField != null && sensitivitySlider != null)
            {
                sensitivityInputField.text = sensitivitySlider.value.ToString("F2");
            }
        }
    }

    private void ApplySensitivity(float value)
    {
        // 適用する直前に、カメラリストが空なら探しに行く
        if (!TryFindAndSetupControllers())
        {
            // まだカメラがない（ロード中など）場合は処理を中断
            return;
        }

        foreach (var axisController in targetControllers)
        {
            if (axisController == null) continue;

            foreach (var controllerEntry in axisController.Controllers)
            {
                bool shouldInvert = false;
                if (invertSettings.ContainsKey(controllerEntry))
                {
                    shouldInvert = invertSettings[controllerEntry];
                }

                float finalValue = shouldInvert ? -value : value;
                controllerEntry.Input.Gain = finalValue;
            }
        }
    }
}