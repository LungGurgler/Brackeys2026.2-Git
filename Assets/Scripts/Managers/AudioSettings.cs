using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance;

    [Header("Audio Mixer")]
    public AudioMixer Mixer;

    public const string KEY_MASTER = "Master";
    public const string KEY_BGM = "BGM";
    public const string KEY_SFX = "SFX";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplySavedVolumesOnStart();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void SetVolumePercent(string key, float percent, bool save = true)
    {
        // Debug.Log("SetSound");
        float db = PercentToDB(percent);
        if (!Mixer.SetFloat(key, db))
        {
            Debug.LogWarning($"Parameter {key} not found in AudioMixer!");
            return;
        }

        if (save)
        {
            PlayerPrefs.SetFloat(key, percent);
            PlayerPrefs.Save();
        }
    }

    private void ApplySavedVolumesOnStart()
    {
        SetVolumePercent(KEY_MASTER, PlayerPrefs.GetFloat(KEY_MASTER, 100f), save: false);
        SetVolumePercent(KEY_BGM, PlayerPrefs.GetFloat(KEY_BGM, 100f), save: false);
        SetVolumePercent(KEY_SFX, PlayerPrefs.GetFloat(KEY_SFX, 100f), save: false);
    }

    private float PercentToDB(float percent)
    {
        if (percent <= 0f) return -80f;
        float normalized = percent / 100f;
        float db;
        if (normalized <= 1f)
            db = Mathf.Lerp(-40f, 0f, Mathf.Log10(1 + 9 * normalized));
        else
            db = Mathf.Lerp(0f, 6f, normalized - 1f);
        return db;
    }

    public bool TryGetVolumePercent(string key, out float percent)
    {
        percent = PlayerPrefs.GetFloat(key, 100f);
        return true;
    }
    public void SetVolume(string key, float percent)
    {
        SetVolumePercent(key, percent, save:true);
    }
}
