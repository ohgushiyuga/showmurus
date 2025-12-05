using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private ParticleSystem flameParticles;
    [SerializeField] private AudioClip flamethowerSound;
    [SerializeField] private AudioSource audioSource;
    private BossActionSettings settings;
    private Transform player;
    private Transform shootPoint;
    private Transform rayOrigin;
    private bool isPlayingByController = false;
    private float lastGroundFireSpawnTime = 0f;
    private float damageTickTimer = 0f;

    void Awake()
    {
        if (flameParticles == null)
        {
            flameParticles = GetComponent<ParticleSystem>();
        }
        flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        audioSource = GetComponent<AudioSource>();

    }

    public void StartFiring(BossActionSettings settings, Transform player, Transform shootPoint, Transform rayOrigin)
    {
        // 必要な参照をすべて受け取る
        this.settings = settings;
        this.player = player;
        this.shootPoint = shootPoint;
        this.rayOrigin = rayOrigin;
        
        this.lastGroundFireSpawnTime = Time.time;
        this.damageTickTimer = 0f; // タイマーリセット
        
        isPlayingByController = true;
        flameParticles.Play();
        AudioManager.Instance.PlaySE("Flamethower");
    }

    public void StopFiring()
    {
        isPlayingByController = false;
        flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        AudioManager.Instance.PlaySE("Flamethower");
    }

    void Update()
    {
        if (!isPlayingByController) return;

        if (player == null || shootPoint == null || rayOrigin == null || settings == null)
        {
            StopFiring();
            return;
        }

        damageTickTimer += Time.deltaTime;

        if (damageTickTimer >= settings.damageTickRate)
        {
            damageTickTimer = 0f;

            FlamethrowerAttackTick();
        }
    }

    private void FlamethrowerAttackTick()
    {
        RaycastHit hit;

        Vector3 rayStartPoint = rayOrigin.position;
        if (Physics.Raycast(rayStartPoint, Vector3.down, out hit, settings.flameRange, settings.floorLayer))
        {
            GenerateGroundFire(hit.point, hit.normal);
        }

        bool hitPlayerDirectly = Physics.SphereCast(
            shootPoint.position,
            settings.flameRadius,
            transform.forward,
            out hit,
            settings.flameRange
        );

        if (hitPlayerDirectly && hit.collider.CompareTag("Player"))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage((int)settings.flameDamage);
                Debug.Log($"火炎放射がプレイヤーに直接ヒット！ダメージ: {settings.flameDamage}");
            }
        }
    }

    void GenerateGroundFire(Vector3 position, Vector3 normal)
    {
        if (Time.time < lastGroundFireSpawnTime + settings.minSpawnInterval)
        {
            return;
        }
        lastGroundFireSpawnTime = Time.time;

        Vector3 spawnPosition = position + normal * 0.01f;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

        GameObject groundFire = Instantiate(settings.groundFirePrefab, spawnPosition, rotation);

        if (groundFire.TryGetComponent(out GroundDamage groundDamageScript))
        {
            groundDamageScript.SetLifetime(settings.groundFireDuration);
            groundDamageScript.damagePerSecond = settings.groundFireDamage;
        }
    }
}