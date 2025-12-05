using UnityEngine;
using UnityEngine.InputSystem;
public class Pause : MonoBehaviour
{
    [SerializeField] public GameObject pauseUI;
    [SerializeField] private PlayerInput playerInput; 

    // 内部状態
    private bool isPaused = false;

    void Start()
    {
        pauseUI.SetActive(false);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    // ポーズ切り替えのメイン処理
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;

        pauseUI.SetActive(true);

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        pauseUI.SetActive(false);

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        isPaused = false;
    }

    // UIの「タイトルへ戻る」ボタン用
    public void GoToTitle()
    {
        Time.timeScale = 1; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}