using UnityEngine;

public class TriggerActivation : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    [SerializeField] private AudioSource bossBGM;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool disableAfterActivation = true;
    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated || bossController == null)
        {
            return;
        }

        if (other.CompareTag(playerTag))
        {
            hasActivated = true;

            bossController.ActivateBoss();
            Debug.Log("ボス起動条件が満たされました: ActivateBoss()を呼び出しました。");

            AudioManager.Instance.PlayBGM("Pursuit_Goes_on");
            Debug.Log("ボスBGMの再生を開始しました。");

            if (disableAfterActivation)
            {
                // トリガーのColliderとこのスクリプトを無効にする（GameObject自体を消すと問題がある場合があるため）
                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
                this.enabled = false;
            }
        }
    }
}