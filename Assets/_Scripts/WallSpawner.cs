using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallSpawner : MonoBehaviour
{
    [Header("生成設定")]
    public List<Transform> spawnPoints;
    public GameObject wallPrefab;
    public float respawnTime = 3f;
    private Dictionary<Transform, GameObject> currentWalls = new Dictionary<Transform, GameObject>();
    
    void Start()
    {
        SpawnAllWalls();
    }
    public void SpawnAllWalls()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            SpawnWall(spawnPoint);
        }
    }

    public void SpawnWall(Transform spawnPoint)
    {
        if (currentWalls.ContainsKey(spawnPoint) && currentWalls[spawnPoint] != null) return;

        GameObject newWall = Instantiate(wallPrefab, spawnPoint.position, spawnPoint.rotation);

        currentWalls[spawnPoint] = newWall;

        if (newWall.TryGetComponent(out DestructibleWall destructibleWall))
        {
            destructibleWall.wallSpawner = this;
            destructibleWall.currentSpawnPoint = spawnPoint;
        }
        currentWalls[spawnPoint] = newWall;
    }

    public void DestroyAndRespawnWall(GameObject wallToDestroy, Transform spawnPoint)
    {
        if (wallToDestroy != null)
        {
            Destroy(wallToDestroy);
            currentWalls[spawnPoint] = null;
        }

        // コルーチンを開始し、指定時間後に壁を再生成する
        StartCoroutine(RespawnAfterDelay(spawnPoint));
    }

    IEnumerator RespawnAfterDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnWall(spawnPoint);
    }
}