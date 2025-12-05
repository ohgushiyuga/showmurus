using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }

    [SerializeField] private string gameOverPrefabName = "Canvas_GameOver";
    [SerializeField] private string startSceneName = "StartScene";
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string gameClearScene = "ClearScene";
    [SerializeField] private float transitionDelay = 5f;
    [SerializeField] private float gameOverPauseDelay =3.5f;
    private GameObject gameOverPanel;
    private bool isTimerRunning = false;
    private bool isGameOver = false;
    private bool isHitStopActive = false;

   void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            ElapsedTime += Time.deltaTime;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            StartCoroutine(InitializeGame());
        }
        else if (scene.name == startSceneName)
        {
            gameOverPanel = null;

            isGameOver = false;
            Time.timeScale = 1f;
            ElapsedTime = 0f;
            isTimerRunning = true;
        }
    }
    
    private IEnumerator InitializeGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        ElapsedTime = 0f;
        isTimerRunning = true;
        yield return null;

        try
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("GameManager: シーン内にCanvasが見つかりません！");
                yield break;
            }
            GameObject prefab = Resources.Load<GameObject>(gameOverPrefabName);
            if (prefab == null)
            {
                Debug.LogError($"GameManager: Resourcesフォルダに '{gameOverPrefabName}' という名前のプレハブが見つかりません！");
                yield break; // コルーチンを停止
            }

            gameOverPanel = Instantiate(prefab, canvas.transform);
            gameOverPanel.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GameManager: UIの生成中にエラー: {e.Message}");
        }
    }

    public void TriggerGameClear()
    {
        if (isGameOver) return;
        isGameOver = true;
        isTimerRunning = false;

        StartCoroutine(LoadSceneAfterDelay(gameClearScene, transitionDelay));
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isTimerRunning = false;

        if (gameOverPanel != null)
        {
            StartCoroutine(GameOverSequence()); 
        }
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(gameOverPauseDelay);

        Time.timeScale = 0f; 

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void StartHitStop(float duration)
    {
        if (isHitStopActive) return; // 既に止まっていたら無視
        StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        isHitStopActive = true;
        
        // 1. 時間を止める
        Time.timeScale = 0f;

        // 2. 現実時間で待機
        yield return new WaitForSecondsRealtime(duration);

        // 3. 時間を元に戻す
        Time.timeScale = 1f;
        
        isHitStopActive = false;
    }
}