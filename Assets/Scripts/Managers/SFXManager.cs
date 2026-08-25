
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    [SerializeField]
    private AudioSource audioSource;
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

        DontDestroyOnLoad(gameObject); //Keeps it persistant between scenes
    }


    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void PlaySFX(SFXKeys key, float pitchRand)
    {
        AudioClip stream = SFXLib.getSoundClip(key);
        if (!stream)
        {
            return;
        }

        audioSource.pitch = 1 + Random.Range(-pitchRand, pitchRand);
        audioSource.PlayOneShot(stream);
    }

    public void PlaySFX(SFXKeys key)
    {
        AudioClip stream = SFXLib.getSoundClip(key);
        if (!stream)
        {
            return;
        }

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(stream);
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

}
