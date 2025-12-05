using UnityEngine;

public class HPDisplayController : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameObject hpCanvas;
    [SerializeField] private float displayRange = 10f;

    private Transform playerTransform;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        if (hpCanvas != null)
        {
            hpCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= displayRange)
        {
            if (!hpCanvas.activeSelf)
            {
                hpCanvas.SetActive(true);
            }

            LookAtCamera(); 
        }
        else
        {
            if (hpCanvas.activeSelf)
            {
                hpCanvas.SetActive(false);
            }
        }
    }
    
    private void LookAtCamera()
    {
        if (Camera.main != null)
        {
            hpCanvas.transform.rotation = Quaternion.LookRotation(hpCanvas.transform.position - Camera.main.transform.position);
        }
    }

    public void UpdateHealthBar(float currentHP, float maxHP, UnityEngine.UI.Image fillImage)
    {
        if (fillImage != null && maxHP > 0)
        {
            float fillAmount = currentHP / maxHP;
            fillImage.fillAmount = fillAmount;
        }
    }
}