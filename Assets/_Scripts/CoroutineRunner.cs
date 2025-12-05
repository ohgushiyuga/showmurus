using UnityEngine;
using System.Collections;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも残るように
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // コルーチンを外部から簡単に開始できるメソッド
    public Coroutine StartRoutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}