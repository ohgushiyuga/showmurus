using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    private GameInput gameInput;
    private PlayerInput playerInput;
    private Rigidbody rb;
    private float currentMoveSpeed;
    public float moveSpeed;
    public float sprintSpeed;
    public float rotateSpeed;
    private Camera mainCamera;
    private Vector2 moveInputValue;
    private bool attack;
    private bool sprint;
    private bool isStunned = false;
    public Animator animator;
    public GunController gunController;
    [SerializeField] AudioClip footStepSound;
    [SerializeField] AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        gameInput = new GameInput();
        playerInput = GetComponent<PlayerInput>();
        SetInputAction();
        gameInput.Enable();
        if (gunController != null && animator != null)
        {
            gunController.playerAnimator = animator;
        }
    }

    private void SetInputAction()
    {
        gameInput.Player.Move.started += OnMove;
        gameInput.Player.Move.performed += OnMove;
        gameInput.Player.Move.canceled += OnMove;

        gameInput.Player.Attack.performed += context => attack = true;
        gameInput.Player.Attack.canceled += context => attack = false;

        gameInput.Player.Sprint.performed += context => sprint = true;
        gameInput.Player.Sprint.canceled += context => sprint = false;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInputValue = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // アニメーション用の速度を計算
        float animationSpeed = 0;

        if(sprint)
        {
            animationSpeed = moveInputValue.magnitude * 2;
            currentMoveSpeed = sprintSpeed; 
        }
        else
        {
            animationSpeed = moveInputValue.magnitude;
            currentMoveSpeed = moveSpeed;
        }

        // アニメーターの "Speed" パラメータに値を渡す
        animator.SetFloat("Speed", animationSpeed, 0.1f, Time.deltaTime); 
       
       if (gunController != null)
        {
             gunController.ReceiveAttackInput(attack);
        }
    }

    private void FixedUpdate()
    {
        if (isStunned || !this.enabled)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        
        Vector3 horDirection = moveInputValue.x * mainCamera.transform.right;
        Vector3 verDirection = moveInputValue.y * mainCamera.transform.forward;

        horDirection.y = 0f;
        verDirection.y = 0f;

        Vector3 moveDirection = (horDirection + verDirection).normalized;
        Vector3 rotateDirection = mainCamera.transform.forward;
        rotateDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = new Vector3(moveDirection.x * currentMoveSpeed,
                                            rb.linearVelocity.y,
                                            moveDirection.z * currentMoveSpeed
                                            );
        }
    }

    public void FootStep()
    {
        AudioManager.Instance.PlaySE("Run");
    }

    public void Death()
    {
        if (animator != null)
        {
            animator.SetBool("Death", true);
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        this.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void ApplyStun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        if (isStunned) yield break;

        isStunned = true;

        if(animator != null)
        {
            animator.SetBool("IsStunned", true);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;

        if(animator)
        {
            animator.SetBool("IsStunned", false);
        }
    }
}
