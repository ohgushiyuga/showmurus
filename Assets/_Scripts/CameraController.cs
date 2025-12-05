using UnityEngine;
using Unity.Cinemachine;
using MyGame;

public class CameraController : MonoBehaviour
{
    private GameInput gameInput;
    public CinemachineCamera tpsCamera;
    public CinemachineCamera fpsCamera;
    private int cameraActive = 0;
    private int cameraInActive = -1;
    private int num = -1;
    private bool change;

    void Awake()
    {
        gameInput = new GameInput();
        InputActions();
        gameInput.Enable();
    }

    void Start()
    {
        tpsCamera.Priority.Value = cameraActive;
        fpsCamera.Priority.Value = cameraInActive;
    }

    private void InputActions()
    {
        gameInput.Camera.Change.performed += context => change = true;
        gameInput.Camera.Change.canceled += context => change = false;
    }

    void Update()
    {
        if (change)
        {
            fpsCamera.Priority.Value = cameraActive;
            tpsCamera.Priority.Value = cameraInActive;
        }
        else
        {
            tpsCamera.Priority.Value = cameraActive;
            fpsCamera.Priority.Value = cameraInActive;
        }
    }
}