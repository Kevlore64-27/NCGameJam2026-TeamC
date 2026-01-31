using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySfx(AudioClip audioClip, float volume = 1.0f)
    {
        StartCoroutine(PlaySfxCoroutine(audioClip, volume));
    }

    IEnumerator PlaySfxCoroutine(AudioClip audioClip, float volume = 1.0f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length * 2);

        Destroy(audioSource);
    }
}
