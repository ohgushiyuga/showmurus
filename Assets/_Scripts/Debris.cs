using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debris : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private GameObject debrisAudioPrefab;
    [SerializeField] private float lifetime = 5.0f; // 破片が消えるまでの時間
    private Collider objectCollider;
    private Rigidbody rb;


    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<BoxCollider>();
        objectCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Barrier"))
        {
            AudioManager.Instance.PlaySE("HitBarrier");

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(character.ATK);
            }
            Destroy(this.gameObject);
        }
        
        if (other.gameObject.CompareTag("Enemy"))
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