using UnityEngine;

[CreateAssetMenu(fileName = "NewBossActionSettings", menuName = "Boss/Action Settings")]
public class BossActionSettings : ScriptableObject
{
    [Header("射撃設定")]
    public GameObject bulletPrefab; //弾のPrefab
    public AudioClip shootSound; //射撃音
    public float shootCooldown = 1f; //射撃間隔
    public int minShootCount = 2; //最小射撃回数
    public int maxShootCount = 5; //最大射撃回数
    public float shootAnimDuration = 0.5f; //射撃アニメーションの硬直時間

    [Header("火炎放射設定")]
    public ParticleSystem flamethrowerEffect; // 火炎放射エフェクトのPrefab
    public GameObject flameAoEWarningPrefab; //火炎放射AoEのPrefab
    public float flamethrowerDuration = 3f; //火炎放射の総持続時間
    public float flameRange = 10f; //火炎放射の射程距離
    public float flameRadius = 4f; //火炎放射の判定の半径（SphereCast用）
    public float flameDamage = 5f; //火炎放射の1ティックあたりのダメージ（直接ダメージ）
    public float damageTickRate = 0.2f; //ティックレート
    public float warningDuration = 1.0f; //警告時間

    [Header("残留炎 (地面) 設定")]
    public GameObject groundFirePrefab; //地面に残す炎のプレハブ
    public LayerMask floorLayer; //地面を判定するレイヤー
    public float groundFireDuration = 20f; //地面残留炎の持続時間
    public float groundFireDamage = 5f; //地面残留炎の1ティックダメージ
    public float minSpawnInterval = 0.2f; //最小生成間隔
    public float lastGroundFireSpawnTime = 0f; // 残留炎を最後に生成した時間

    [Header("移動と特殊攻撃")]
    public GameObject stompEffect; //スタンプのエフェクト
    public GameObject stompAoEWarningPrefab; //スタンプAoEのPrefab
    public AudioClip stompSound; //スタンプ攻撃の効果音
    public float stompRange = 5f; // スタンプ攻撃の範囲
    public float stompAoEForwardOffset = 2.0f; //足元から前方に
    public float stompDamage = 5f; //スタンプ攻撃のダメージ
    public float stunDuration = 1f; //スタン時間
    public float stompAnimDuration = 1f; //アニメーション時間
    public float stompShakeForce = 3.0f; // シェイクする強さ
    public float stompHitStopDuration = 0.1f; // シェイクする時間
    public float moveSpeed = 3f; // 移動速度
    public float movingStateTimer = 0f; // 移動状態の残り時間
}