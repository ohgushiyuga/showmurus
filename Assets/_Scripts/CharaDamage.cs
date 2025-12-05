using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private Character character;
    [SerializeField] private DamageEffect damageEffect;
    [SerializeField] private Slider slider;
    [SerializeField]private bool isBoss = false;
    private PlayerController playerController;
    private DestructibleWall dWall;
    private DamageFlasher damageFlasher;
    public Animator animator;
    private int currentHP;

    void Start()
    {
        if (character != null)
        {
            currentHP = character.MAXHP;
        }

        if(slider != null)
        {
            slider.maxValue = character.MAXHP;
            slider.value = currentHP;
        }
    }
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        dWall = GetComponent<DestructibleWall>();
        damageFlasher = GetComponent<DamageFlasher>();
        animator = GetComponent<Animator>();
    }

    public void Damage(int value)
    {
        int damageTaken = value - character.DEF;

        if(damageTaken > 0)
        {
            currentHP -= damageTaken;
        }

        if (damageEffect != null)
        {
            damageEffect.PlayDamageEffect();
        }

        if (damageFlasher != null)
        {
            damageFlasher.Flash();
        }

        slider.value = currentHP;
                
        if (currentHP <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        animator.SetBool("Death", true);

        if(isBoss && GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameClear();
        }

        if (playerController != null)
        {
            playerController.Death();
            GameManager.Instance.TriggerGameOver();
        }

        if(dWall != null)
        {
            dWall.Death();
        }
    }

    public void DeathEnd()
    {
        this.gameObject.SetActive(false);
    }
}