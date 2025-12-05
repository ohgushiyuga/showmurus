using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public Animator playerAnimator;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float timeBetweenShooting;     // 発射レート
    [SerializeField] float reloadTime;              // リロード時間
    [SerializeField] int magazineSize;             // マガジンのサイズ
    [SerializeField] float bulletForce;
    [SerializeField] AudioClip gunShotSound;
    [SerializeField] AudioSource audioSource;

    private Camera mainCamera;

    int bulletsShot, bulletsLeft;
    private bool isReadyToShoot;
    public bool isReloading;
    private bool isWaitingForReloadFinish = false;
    public AmmoDisplayController ammoDisplayController;
    public Text ammo;

    void Start()
    {
        bulletsLeft = magazineSize;
        isReadyToShoot = true;
        mainCamera = Camera.main;
        if (ammoDisplayController != null)
        {
            ammoDisplayController.UpdateAmmoDisplay(bulletsLeft);
        }
        ammo.text = "弾:" + bulletsLeft;
        audioSource = firePoint.GetComponent<AudioSource>();
    }

    public void ReceiveAttackInput(bool isAttacking)
    {
        if (isReloading || isWaitingForReloadFinish)
        {
            return;
        }
    
        // 弾が切れた状態で、攻撃が試みられた場合
        if (isAttacking && bulletsLeft <= 0)
        {
            // 攻撃ボタンを押し続けている間、リロードが無限に呼ばれるのを防ぐため、フラグを立ててからリロードを開始する。
            isWaitingForReloadFinish = true;
            Reload();
            return;
        }
    
        // 攻撃ボタンが押されており、かつ撃てる状態、リロード中でなく、弾が残っているか
        if (isAttacking && isReadyToShoot && !isReloading && bulletsLeft > 0)
        {
            Shot();
        } 
    }

    private void Shot()
    {
        isReadyToShoot = false;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Shoot");
        }

        AudioManager.Instance.PlaySE("PlayerShot");

        GameObject currentBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Bulletに発射を命令
        if (currentBullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.Fire(mainCamera.transform, bulletForce); 
        }

        bulletsLeft--;

        ammoDisplayController.UpdateAmmoDisplay(bulletsLeft);
        ammo.text = "弾:" + bulletsLeft;

        // 発射レートのクールダウン
        Invoke(nameof(ResetShot), timeBetweenShooting);
    }

    // 撃てる状態にする
    private void ResetShot()
    {
        isReadyToShoot = true;
    }

    // リロード処理
    private void Reload()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Reload");
        }
        
        isReloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    // リロード状態を終了
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        isReloading = false;

        ammoDisplayController.UpdateAmmoDisplay(bulletsLeft);
        ammo.text = "弾" + bulletsLeft;

        isWaitingForReloadFinish = false;
    }
}