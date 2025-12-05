// GameTimer.cs

using UnityEngine;
using UnityEngine.UI; // 標準UI Text用
using TMPro; // TextMeshPro用

public class GameTimer : MonoBehaviour
{
    [Header("タイマー設定")]
    [SerializeField] private float totalTime = 180f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CharaDamage playerCharaDamage;

    // 内部変数
    private float currentTime;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = totalTime;
        isGameOver = false;

        if (playerCharaDamage == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerCharaDamage = player.GetComponent<CharaDamage>();
        }
    }

    void Update()
    {
        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            TriggerGameOver();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        // 時間を「分：秒」の形式にフォーマット
        // (例: 180秒 -> 03:00)
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("時間切れ！ゲームオーバー。");

        if (playerCharaDamage != null)
        {
            playerCharaDamage.Death();
        }
    }
}