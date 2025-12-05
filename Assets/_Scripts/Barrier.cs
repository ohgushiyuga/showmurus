using UnityEngine;
using UnityEngine.UI;

public class Barrier : MonoBehaviour, IDamageable 
{
    [Header("バリアの耐久設定")]
    [SerializeField] private Character character;
    [SerializeField] private BossController bossController;
    [SerializeField] Slider slider;
    public GameObject barrierAudioPrefab;
    public string allowedTag = "Enemy";
    public float pushForce = 100f;
    private int currentHealth;

    void Start()
    {
        currentHealth = character.MAXHP;

        slider.value = 1;
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (other.CompareTag(allowedTag))
        {
            return;
        }

        Vector3 pushDirection = other.transform.position - transform.position;

        pushDirection.y = 0;

        pushDirection = pushDirection.normalized;

        rb.AddForce(pushDirection * pushForce, ForceMode.Force);
    }

    public void Damage(int value)
    {
        int damageTaken = value - character.DEF;

        if (damageTaken > 0)
        {
            currentHealth -= damageTaken;

            slider.value = (float)currentHealth / (float)character.MAXHP;
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        AudioManager.Instance.PlaySE("BreakBarrier");
        //Instantiate(barrierAudioPrefab, transform.position, transform.rotation);
        
        if (bossController != null)
        {
            bossController.BreakBarrier();
        }
        else
        {
            Debug.LogError("EnemyControllerがアタッチされてません");
        }
        Destroy(gameObject);
    }
}