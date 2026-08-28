 using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance;

    [SerializeField]
    float fadeDuration = 1.5f;

    private MusicKeys currentKey = MusicKeys.Null;
    private bool AudioPlaying = false;

    [SerializeField]
    AudioSource audioSource;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject); 

    }

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        playAudio(MusicKeys.PlaceholderBGM);
    }
    private void Update()
    {

    }

    public void playAudio(MusicKeys audioKey)
    {


        if (currentKey == audioKey)
        {
            return;
        }

        currentKey = audioKey;

        StopAllCoroutines();

        if (AudioPlaying)
        {
            StartCoroutine(OutFadeAudio(audioKey));
        }
        else
        {
            AudioPlaying = true;
            changeStream(audioKey);
        }

    }

    public void StopAudio()
    {
        audioSource.Stop();
        AudioPlaying = false;
    }

    private IEnumerator OutFadeAudio(MusicKeys musicKey)
    {
        float elapsedTime = 0f;
        float max = audioSource.volume;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(max, 0, elapsedTime / fadeDuration);
            yield return new WaitForEndOfFrame();
        }

        audioSource.Stop();
        audioSource.volume = 1f;
        changeStream(musicKey);

    }

    private void changeStream(MusicKeys musicKey)
    {

        audioSource.clip = MusicLib.getMusicClip(musicKey);
        audioSource.Play();
        audioSource.loop = true;
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }

}

