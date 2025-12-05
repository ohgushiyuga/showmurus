using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    private AudioSource audioSource;
    bool isAudioPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        isAudioPlaying = true;
    }

    void Update()
    {
        if (!audioSource.isPlaying && isAudioPlaying)
        {
            Destroy(gameObject);
        }
    }
}