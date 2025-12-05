using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour
{
    public enum VolumeType
    {
        BGM,
        SE
    }

    [Header("設定")]
    public VolumeType volumeType;
    public TMP_InputField inputField;

    [Header("デフォルト設定")]
    [SerializeField] private float defaultVolume = 1.0f;

    private Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManagerがシーンに存在しません！");
            return;
        }

        // 保存値の取得（なければdefaultVolumeを使う）
        float currentVol = defaultVolume;
        if (volumeType == VolumeType.BGM)
        {
            currentVol = PlayerPrefs.GetFloat(AudioManager.BGM_VOLUME_KEY, defaultVolume);
        }
        else
        {
            currentVol = PlayerPrefs.GetFloat(AudioManager.SE_VOLUME_KEY, defaultVolume);
        }

        // スライダー初期化
        _slider.value = currentVol;
        _slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Input Field初期化
        if (inputField != null)
        {
            inputField.text = (currentVol * 100f).ToString("F0");
            inputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
    }

    // デフォルトボタンから呼ぶ関数
    public void OnClickDefault()
    {
        if (_slider != null)
        {
            // スライダーの値をデフォルトに戻す
            // 自動的に OnSliderValueChanged が呼ばれ、音量・表示・保存が全て更新されます
            _slider.value = defaultVolume;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        // 音量適用
        if (volumeType == VolumeType.BGM)
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
        else
        {
            AudioManager.Instance.SetSEVolume(value);
        }

        // 表示更新 (0～100)
        string displayValue = (value * 100f).ToString("F0");
        if (inputField != null && inputField.text != displayValue)
        {
            inputField.text = displayValue;
        }
    }

    private void OnInputFieldEndEdit(string text)
    {
        if (float.TryParse(text, out float inputValue))
        {
            // 0～100 の入力を 0.0～1.0 に変換
            float vol0to1 = inputValue / 100f;
            vol0to1 = Mathf.Clamp(vol0to1, 0f, 1f);

            _slider.value = vol0to1;
        }

        if (inputField != null)
        {
            inputField.text = (_slider.value * 100f).ToString("F0");
        }
    }
}