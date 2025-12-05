using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AmmoDisplayController : MonoBehaviour
{
    public Transform ammoIconParent;
    public GameObject[] ammoIcons;
    private int maxAmmo;

    void Start()
    {
        // 親オブジェクトから全ての子アイコンを取得し、配列に格納
        maxAmmo = ammoIconParent.childCount;
        ammoIcons = new GameObject[maxAmmo];
        for (int i = 0; i < maxAmmo; i++)
        {
            ammoIcons[i] = ammoIconParent.GetChild(i).gameObject;
        }

        // Horizontal Layout Groupが設定されているか確認
        if (ammoIconParent.GetComponent<HorizontalLayoutGroup>() == null)
        {
            Debug.LogWarning("AmmoIconParentにHorizontal Layout Groupがありません。中央揃えのため設定を推奨します。");
        }
    }

    /// <param name="currentAmmo">現在の残弾数</param>
    public void UpdateAmmoDisplay(int currentAmmo)
    {
        if (currentAmmo > maxAmmo) currentAmmo = maxAmmo;

        for (int i = 0; i < maxAmmo; i++)
        {
            // インデックス (i) が現在の残弾数より小さい場合 (i < currentAmmo) は表示 (満タン弾)
            // それ以外 (i >= currentAmmo) は非表示 (使用済み弾)
            // もし、使用済み弾のアイコンを表示したい場合は、SetActive(false)の代わりに空の弾アイコンのImageを有効にする
            bool isVisible = i < currentAmmo;

            ammoIcons[i].SetActive(isVisible);
        }
    }
}