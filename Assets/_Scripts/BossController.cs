using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class BossController : MonoBehaviour
{
    // 内部状態
    private enum EnemyState
    {
        Shooting,       // 射撃中. #1
        Flamethrower,   // 火炎放射中. #2 
        Moving,         // 移動中. #3
        Stomping,       // スタンプ攻撃中 #4
        Idle,           // 待機. #5
    }

    [Header("ボスの状態")]
    public bool hasBarrier = true;  // バリアの有無
    private bool isActivated = false;  // 起動状態
    [Header("参照")]
    [SerializeField] private BossActionSettings settings; //行動設定
    [SerializeField] private Transform player;  // プレイヤーの参照
    [SerializeField] private Transform shootPoint;  // 射撃位置
    [SerializeField] private Transform rayOrigin; //残留炎の位置
    [SerializeField] private Flamethrower flamethrowerScript; //火炎放射スクリプトの参照
    [SerializeField] private Animator animator;  // アニメーター
    [SerializeField] private AudioSource audioSource; //スピーカー的役割
    private EnemyState currentState = EnemyState.Moving;
    private CinemachineImpulseSource impulseSource;
    private int remainingShots = 0;
    private bool isActionInProgress = false;

    void Start()
    {
        isActivated = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        if (settings == null)
        {
            Debug.LogError("BossActionSettingsがInspectorで設定されていません");
            isActivated = false;
            return;
        }

        if (settings.flamethrowerEffect != null)
        {
            settings.flamethrowerEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (!isActivated || player == null) return;

        if (currentState != EnemyState.Moving && currentState != EnemyState.Stomping) //Swith文で書いたほうがいいかも
        {
            LookAtPlayer();
        }

        if (currentState == EnemyState.Moving)
        {
            HandleMovingState();
        }
    }

    // ボスを起動する
    public void ActivateBoss()
    {
        isActivated = true;
        StartShootingPattern();
    }

    // バリアを破壊した時に呼ぶ
    public void BreakBarrier()
    {
        hasBarrier = false;
        Debug.Log("バリアを破壊！ボスが動き出す！");
    }

    // === 状態処理 ===

    // 射撃パターンを開始するが、完了をコルーチンで待つ
    void StartShootingPattern()
    {
        if (isActionInProgress) return;
        isActionInProgress = true;
        StartCoroutine(ShootingPatternCoroutine());
    }

    IEnumerator ShootingPatternCoroutine()
    {
        currentState = EnemyState.Shooting;
        remainingShots = Random.Range(settings.minShootCount, settings.maxShootCount + 1);

        animator.SetBool("isShooting", true);
        animator.SetBool("isWalking", false);

        while (remainingShots > 0)
        {
            Shoot();
            yield return new WaitForSeconds(settings.shootAnimDuration);

            yield return new WaitForSeconds(settings.shootCooldown - settings.shootAnimDuration);

            remainingShots--;
        }

        StartCoroutine(FlamethrowerCoroutine());
    }

    IEnumerator FlamethrowerCoroutine()
    {
        currentState = EnemyState.Flamethrower;
        Debug.Log("火炎放射開始");

        animator.SetBool("isShooting", false);
        animator.SetBool("isFlamethrowing", true);

        GameObject warningInstance = null;

        Vector3 spawnPosition = transform.position + transform.forward * settings.stompAoEForwardOffset;

        spawnPosition.y = transform.position.y;

        warningInstance = Instantiate(settings.flameAoEWarningPrefab, spawnPosition, transform.rotation);

        

        warningInstance.transform.SetParent(this.transform);

        if (settings.flamethrowerEffect != null)
        {
            settings.flamethrowerEffect.Play();

            if (flamethrowerScript != null)
            {
                flamethrowerScript.StartFiring(settings, player, shootPoint, rayOrigin);
            }
        }

        yield return new WaitForSeconds(settings.flamethrowerDuration);

        StopFlamethrower();

        if (warningInstance != null)
        {
            Destroy(warningInstance);
        }
        
        if (!hasBarrier)
        {
            currentState = EnemyState.Moving;
            isActionInProgress = false;
            settings.movingStateTimer = 5f;
        }
        else
        {
            StartCoroutine(ShootingPatternCoroutine());
        }
    }

    void HandleMovingState()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= settings.stompRange)
        {
            StartCoroutine(StompAttackCoroutine());
            return;
        }

        animator.SetBool("isWalking", true);

        // プレイヤーに向かって移動
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * settings.moveSpeed * Time.deltaTime;

        // プレイヤーの方を向く
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        settings.movingStateTimer -= Time.deltaTime;
        if (settings.movingStateTimer <= 0f)
        {
            isActionInProgress = true;
            StartCoroutine(ShootingPatternCoroutine());
        }
    }

    // スタンプ攻撃のコルーチン
    IEnumerator StompAttackCoroutine()
    {
        currentState = EnemyState.Stomping;
        isActionInProgress = true;
        Debug.Log("スタンプ攻撃");

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = targetRotation;

        GameObject warningInstance = null;
        Vector3 spawnPosition = transform.position + transform.forward * settings.stompAoEForwardOffset;
        spawnPosition.y = transform.position.y;

        warningInstance = Instantiate(settings.stompAoEWarningPrefab, spawnPosition, transform.rotation);
        float targetDiameter = settings.stompRange * 2f;
        warningInstance.transform.localScale = Vector3.one * targetDiameter;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("TriggerStomp");

        yield return new WaitForSeconds(settings.warningDuration);

        if (warningInstance != null)
        {
            Destroy(warningInstance);
        }

        float remainingWaitTime = settings.stompAnimDuration - settings.warningDuration;
        if (remainingWaitTime > 0)
        {
            yield return new WaitForSeconds(remainingWaitTime);
        }

        Vector3 stompCenter = transform.position + transform.forward * settings.stompAoEForwardOffset;
        stompCenter.y = transform.position.y;

        AudioManager.Instance.PlaySE("Stomp");

        if (settings.stompEffect != null)
        {
            Instantiate(settings.stompEffect, stompCenter, Quaternion.identity);
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(settings.stompShakeForce);
        }

        // ヒットストップ
        if (GameManager.Instance != null && settings.stompHitStopDuration > 0)
        {
            GameManager.Instance.StartHitStop(settings.stompHitStopDuration);
        }

        // アニメーション終了後、ダメージ判定
        Collider[] hitColliders = Physics.OverlapSphere(stompCenter, settings.stompRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("プレイヤーにダメージとスタンを適用");

                PlayerController playerController = hitCollider.GetComponent<PlayerController>();

                IDamageable damageable = hitCollider.GetComponent<IDamageable>();

                damageable.Damage((int)settings.stompDamage);

                playerController.ApplyStun(settings.stunDuration);
            }
        }

        StartCoroutine(ShootingPatternCoroutine());
    }
    
    // 実際の攻撃処理

    void Shoot()
    {
        Debug.Log("射撃");

         AudioManager.Instance.PlaySE("BossShot");

        if (settings.bulletPrefab != null && shootPoint != null)
        {
            // プレイヤーの方向に弾を発射
            Vector3 direction = (player.position - shootPoint.position).normalized;
            GameObject bullet = Instantiate(settings.bulletPrefab, shootPoint.position, Quaternion.identity);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.linearVelocity = direction * 100f;
        }
    }

    void StopFlamethrower()
    {
        Debug.Log("火炎放射終了");

        animator.SetBool("isFlamethrowing", false);

        if (settings.flamethrowerEffect != null)
        {
            settings.flamethrowerEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (flamethrowerScript != null)
            {
                flamethrowerScript.StopFiring();
            }
        }
    }

    // プレイヤーの方向を向く処理
    private void LookAtPlayer()
    {
        if (player == null) return;

        // プレイヤーへの方向ベクトル
        Vector3 direction = (player.position - transform.position);

        direction.y = 0;

        if (direction == Vector3.zero) return;

        // ターゲットの水平回転
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 10f
        );
    }
}