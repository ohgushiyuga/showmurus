using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RayCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private float extraRayDistance = 2.0f; 
    [SerializeField] private float backwardDistance = 1.0f;
    [SerializeField] private float cameraCheckRadius = 0.5f;

    private List<GameObject> currentHitObjects = new List<GameObject>();
    private List<GameObject> prevHitObjects = new List<GameObject>();

    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Rayによる検知（カメラとプレイヤーの間）
        Vector3 rayOrigin = transform.position - (direction * backwardDistance);
        float totalRayLength = distance + extraRayDistance + backwardDistance;
        Ray ray = new Ray(rayOrigin, direction);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, totalRayLength, obstacleLayer);

        // Sphereによる検知（カメラ自身の埋まり防止）
        // カメラの位置を中心に、半径 cameraCheckRadius の球体判定を行う
        Collider[] sphereHits = Physics.OverlapSphere(transform.position, cameraCheckRadius, obstacleLayer);

        Debug.DrawRay(rayOrigin, direction * totalRayLength, Color.red);

        // --- リスト更新処理 ---

        prevHitObjects = new List<GameObject>(currentHitObjects);
        currentHitObjects.Clear();

        // Rayで当たったものを追加
        AddHitsToCurrentList(rayHits);

        // Sphereで当たったものを追加
        AddSphereHitsToCurrentList(sphereHits);

        // 以前当たっていたが、今は当たっていないものを元に戻す
        foreach (GameObject obj in prevHitObjects.Except(currentHitObjects))
        {
            if (obj != null)
            {
                TranslucentMaterial translucent = obj.GetComponent<TranslucentMaterial>();
                if (translucent != null)
                {
                    translucent.NotClearMaterialInvoke();
                }
            }
        }
    }

    // RaycastHit配列用の追加処理
    void AddHitsToCurrentList(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            TranslucentMaterial translucent = hit.collider.GetComponentInParent<TranslucentMaterial>();
            AddTranslucentObject(translucent);
        }
    }

    // Collider配列用の追加処理（OverlapSphere用）
    void AddSphereHitsToCurrentList(Collider[] hits)
    {
        foreach (Collider col in hits)
        {
            TranslucentMaterial translucent = col.GetComponentInParent<TranslucentMaterial>();
            AddTranslucentObject(translucent);
        }
    }

    // 共通の登録処理
    void AddTranslucentObject(TranslucentMaterial translucent)
    {
        if (translucent != null)
        {
            if (IsObstacleTag(translucent.gameObject.tag)) // タグ判定はスクリプトのアタッチ先で見る
            {
                translucent.ClearMaterialInvoke();

                if (!currentHitObjects.Contains(translucent.gameObject))
                {
                    currentHitObjects.Add(translucent.gameObject);
                }
            }
        }
    }

    private bool IsObstacleTag(string tagName)
    {
        // ここに透過させたいタグを追加
        return tagName == "Wall" || tagName == "Floor"; // 必要に応じて追加
    }
}