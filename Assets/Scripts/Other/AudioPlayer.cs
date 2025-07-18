using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume)
    {
        m_AudioSource.clip = clip;
        m_AudioSource.volume = volume;
        m_AudioSource.Play();
        StartCoroutine(EndOfPlay());    
    }

    private IEnumerator EndOfPlay()
    {
        // Wait until the audio has finished playing
        yield return new WaitWhile(() => m_AudioSource.isPlaying);

        // Optional: Short safety delay just in case of rounding issues
        yield return new WaitForSeconds(0.1f);

        // Destroy the GameObject this script is attached to
        Destroy(gameObject);
    }

}
