using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class DestructibleWall : MonoBehaviour, IDamageable
{
    [HideInInspector] public WallSpawner wallSpawner; 
    [HideInInspector] public Transform currentSpawnPoint;

    [Header("壁の耐久設定")]
    [SerializeField] private Character character;

    [Header("破壊と飛散設定")]
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 1f;
    [SerializeField] private float launchForce = 1500;

    [Header("破壊エフェクト（Juice）")]
    [SerializeField] private float hitStopDuration = 0.05f;
    [SerializeField] private float shakeForce = 1.0f;
    
    [Header("往復移動設定")]
    [SerializeField] private float topYOffset = 3.0f; //目標上Y座標
    [SerializeField] private float bottomYOffset = 0.0f; //目標下Y座標
    [SerializeField] private float minMoveSpeed = 1.5f; //移動速度最小値
    [SerializeField] private float maxMoveSpeed = 3.0f; //移動速度最大値
    [SerializeField] private float minWaitTime = 1.0f; //停止時間最小値
    [SerializeField] private float maxWaitTime = 4.0f; //停止時間最大値
    [SerializeField] private float maxStartDelay = 2.0f; //遅延時間の最大値

    // 内部状態
    private Transform playerCameraTransform;
    private Vector3 initialPosition; // StartPosを初期位置として保持
    private Coroutine movementCoroutine; // 移動コルーチンの参照を保持
    private CinemachineImpulseSource impulseSource;
    private int currentHealth;

    void Start()
    {
        currentHealth = character.MAXHP;
        initialPosition = transform.position;
        
        if (Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();

        StartMovementLogic();
    }

    // 移動処理の開始関数
    void StartMovementLogic()
    {
        // 既存のコルーチンを停止してから開始（再生成された場合のため）
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        
        float startDelay = Random.Range(0f, maxStartDelay);
        movementCoroutine = StartCoroutine(MoveAndWaitWithDelay(startDelay));
    }
    
    // 初期遅延処理を含むコルーチン
    IEnumerator MoveAndWaitWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        movementCoroutine = StartCoroutine(MoveAndWait());
    }

    // 移動と待機を繰り返すメインのコルーチン
    IEnumerator MoveAndWait()
    {
        Vector3 targetPosition = initialPosition;
        targetPosition.y += bottomYOffset;
        
        while (true)
        {
            if (targetPosition.y == initialPosition.y + bottomYOffset)
            {
                targetPosition.y = initialPosition.y + topYOffset;
            }
            else
            {
                targetPosition.y = initialPosition.y + bottomYOffset;
            }
            
            float currentMoveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
            
            // 目標地点に到達するまで移動
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    currentMoveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;

            float currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(currentWaitTime);
        }
    }

    public void Damage(int value)
    {
        int damageTaken = value - character.DEF;

        if (damageTaken > 0)
        {
            currentHealth -= damageTaken;
        }

        if (currentHealth <= 0)
        {
            DeathAndNotifySpawner();
        }
    }

    public void DeathAndNotifySpawner()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);

        AudioManager.Instance.PlaySE("BreakWall");

        // カメラーシェイク発生
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(shakeForce);
        }

        if (hitStopDuration > 0 && GameManager.Instance != null)
        {
            // 壁が消えてもGameManagerは生きているので、時間が動き出します
            GameManager.Instance.StartHitStop(hitStopDuration);
        }

        // 破片生成と飛散
        GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
        ExplodeDebris(debris);

        // スポナー通知
        if (wallSpawner != null)
        {
            wallSpawner.DestroyAndRespawnWall(gameObject, currentSpawnPoint);
        }
        else
        {
            Destroy(gameObject);
        }
    }

     private void ExplodeDebris(GameObject debrisContainer)
    {
        Rigidbody[] fragments = debrisContainer.GetComponentsInChildren<Rigidbody>();

        Vector3 cameraForwardDirection = Vector3.zero;

        if (playerCameraTransform != null)
        {
            cameraForwardDirection = playerCameraTransform.forward;
        }

        foreach (Rigidbody rb in fragments)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.WakeUp();

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.0f, ForceMode.Impulse);

                if (cameraForwardDirection != Vector3.zero)
                {
                    rb.AddForce(cameraForwardDirection * launchForce, ForceMode.Impulse);
                }
                rb.AddTorque(Random.insideUnitSphere * explosionForce * 0.1f);
            }
        }
        if (debrisContainer.GetComponent<Debris>() == null)
        {
            debrisContainer.AddComponent<Debris>();
        }
    }


    public void Death()
    {
        DeathAndNotifySpawner();
    }
}