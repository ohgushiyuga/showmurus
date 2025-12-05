using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDamage : MonoBehaviour
{
    [Header("ダメージ設定")]
    [Tooltip("毎秒与える基本ダメージ量")]
    public float damagePerSecond = 10.0f;
    
    // 内部タイマー制御
    private float damageInterval = 0.5f; // 0.5秒ごとにダメージを与える
    private float damageTimer = 0.0f;

    // ゾーン内にいるダメージ可能なオブジェクトのリスト
    private List<IDamageable> objectsInZone = new List<IDamageable>();
    
    // 寿命を外部から設定するための変数
    private float lifetime = 30f; 

    // BossControllerから寿命を設定する公開関数
    public void SetLifetime(float duration)
    {
        lifetime = duration;
        // ゾーンの寿命を設定
        Destroy(gameObject, lifetime);
    }
    
    // --- メインロジック ---

    void Update()
    {
        // ダメージタイマーの更新
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            // ダメージ間隔が経過したらダメージを与える
            ApplyDamageToZone();
            damageTimer = damageInterval; // タイマーをリセット
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーまたは敵に IDamageable があるかチェック
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) //ボスに炎ダメージを受けさせたくなければここを消す
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            
            if (!objectsInZone.Contains(damageable))
            {
                objectsInZone.Add(damageable);
            }
            else
            {
                Debug.LogError("damagebleが取得できていません");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ゾーンから出たオブジェクトをリストから削除
        if (other.TryGetComponent(out IDamageable damageable))
        {
            objectsInZone.Remove(damageable);
        }
    }

    private void ApplyDamageToZone()
    {
        // 現在ゾーン内にいるすべてのオブジェクトに対してダメージを適用
        for (int i = objectsInZone.Count - 1; i >= 0; i--)
        {
            IDamageable target = objectsInZone[i];

            // オブジェクトがnullでなければダメージを与える
            if (target != null)
            {
                // 1回のダメージ量 = (damagePerSecond / (1秒 / damageInterval))
                float damageAmount = damagePerSecond * damageInterval;
                
                target.Damage(Mathf.RoundToInt(damageAmount));
            }
            else
            {
                // オブジェクトが破壊された場合はリストから削除
                objectsInZone.RemoveAt(i);
            }
        }
    }
}