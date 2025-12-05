using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    private Collider objectCollider;
    [SerializeField] private Character character;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<SphereCollider>();
        objectCollider.isTrigger = true; //Triggerとして扱う
    }

    public void Fire(Transform bulletTransform, float force)
    {
        // BulletControllerから受け取った力で発射
        rb.AddForce(bulletTransform.forward * force, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Block")) //タグがBlockのオブジェクトと衝突した場合
        {
            Destroy(this.gameObject); //弾を消す
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(character.ATK);
            }
            
            Destroy(this.gameObject);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {

            AudioManager.Instance.PlaySE("HitRobot");
            
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(character.ATK);
            }

            Destroy(this.gameObject);
        }
        
        if (other.gameObject.CompareTag("Barrier"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(character.ATK);
            }

            Destroy(this.gameObject);
        }
    }
}
