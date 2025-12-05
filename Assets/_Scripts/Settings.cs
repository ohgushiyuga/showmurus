using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class settings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] AudioMixer audioMixer;

    [Header("BGM Settings")]
    [SerializeField] Slider BGMSlider;
    public TMP_InputField BGMInputField;

    [Header("SE Settings")]
    [SerializeField] Slider SESlider;
    public TMP_InputField SEInputField;

    private void Start()
    {
        // --- BGMの初期化 ---
        if (audioMixer.GetFloat("BGM", out float bgmVolume))
        {
            BGMSlider.value = bgmVolume;
            // 初期値をテキストにも反映
            if (BGMInputField != null)
                BGMInputField.text = bgmVolume.ToString("F0"); // F0は整数表示。小数がいいならF2など
        }

        // BGM入力フィールドのイベント登録
        if (BGMInputField != null)
            BGMInputField.onEndEdit.AddListener(OnBGMInputEndEdit);


        // --- SEの初期化 ---
        if (audioMixer.GetFloat("SE", out float seVolume))
        {
            SESlider.value = seVolume;
            if (SEInputField != null)
                SEInputField.text = seVolume.ToString("F0");
        }

        // SE入力フィールドのイベント登録
        if (SEInputField != null)
            SEInputField.onEndEdit.AddListener(OnSEInputEndEdit);
    }

    // BGM用の処理

    // スライダーを動かした時に呼ぶ関数（InspectorのOnValueChangedに設定、または自動で呼ばれる）
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);

        // テキストを更新（無限ループ防止チェック付き）
        if (BGMInputField != null && BGMInputField.text != volume.ToString("F0"))
        {
            BGMInputField.text = volume.ToString("F0");
        }
    }

    // InputFieldの入力が終わった時に呼ぶ関数
    public void OnBGMInputEndEdit(string text)
    {
        if (float.TryParse(text, out float value))
        {
            // スライダーの範囲内に収める
            if (BGMSlider != null)
            {
                value = Mathf.Clamp(value, BGMSlider.minValue, BGMSlider.maxValue);
                BGMSlider.value = value; // これで SetBGM も自動的に呼ばれます
            }
        }
        else
        {
            // 数値以外なら元に戻す
            if (BGMSlider != null && BGMInputField != null)
                BGMInputField.text = BGMSlider.value.ToString("F0");
        }
    }


    // SE用の処理

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
        
        // テキスト更新
        if (SEInputField != null && SEInputField.text != volume.ToString("F0"))
        {
            SEInputField.text = volume.ToString("F0");
        }

        // 確認用の音を鳴らすならここ（負荷軽減のため頻度に注意）
    }

    public void OnSEInputEndEdit(string text)
    {
        if (float.TryParse(text, out float value))
        {
            if (SESlider != null)
            {
                value = Mathf.Clamp(value, SESlider.minValue, SESlider.maxValue);
                SESlider.value = value;
            }
        }
        else
        {
            if (SESlider != null && SEInputField != null)
                SEInputField.text = SESlider.value.ToString("F0");
        }
    }
}