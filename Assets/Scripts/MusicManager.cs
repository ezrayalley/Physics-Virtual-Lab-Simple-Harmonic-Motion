using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    public float startDelay = 20f; // delay in seconds

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;

            StartCoroutine(PlayMusicWithDelay());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator PlayMusicWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        audioSource.Play();
    }
}