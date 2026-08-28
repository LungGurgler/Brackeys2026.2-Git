using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider MasterVolume_Slider;
    public Slider BGM_Slider;
    public Slider SFX_Slider;

    [Header("Volume Labels")]
    public TextMeshProUGUI MasterVolume_Text;
    public TextMeshProUGUI BGM_Text;
    public TextMeshProUGUI SFX_Text;

    [Header("Audio Mixer")]
    public AudioMixer Mixer;

    void Start()
    {
        float savedMaster = PlayerPrefs.GetFloat("Master", 100f);
        float savedBGM = PlayerPrefs.GetFloat("BGM", 100f);
        float savedSFX = PlayerPrefs.GetFloat("SFX", 100f);

        if (MasterVolume_Slider != null) MasterVolume_Slider.value = savedMaster;
        if (BGM_Slider != null) BGM_Slider.value = savedBGM;
        if (SFX_Slider != null) SFX_Slider.value = savedSFX;

        UpdateVolume(MasterVolume_Text, savedMaster, "Master");
        UpdateVolume(BGM_Text, savedBGM, "BGM");
        UpdateVolume(SFX_Text, savedSFX, "SFX");

        if (MasterVolume_Slider != null)
        {
            MasterVolume_Slider.onValueChanged.AddListener((v) =>
            {
                UpdateVolume(MasterVolume_Text, v, "Master");
                ClampBGMAndSFXToMaster();
            });
        }

        if (BGM_Slider != null)
        {
            BGM_Slider.onValueChanged.AddListener((v) => UpdateVolume(BGM_Text, v, "BGM"));
        }

        if (SFX_Slider != null)
        {
            SFX_Slider.onValueChanged.AddListener((v) =>
            {
                UpdateVolume(SFX_Text, v, "SFX");
            });

            AddPointerUpListener(SFX_Slider.gameObject, OnSFXSliderReleased);
        }
    }

    // some Gemini yap that fires a trigger when the SFX Slider is released
    private void AddPointerUpListener(GameObject targetObject, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = targetObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = targetObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };

        entry.callback.AddListener((data) => { action.Invoke(); });
        trigger.triggers.Add(entry);
    }

    private void OnSFXSliderReleased()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SFXKeys.MenuBtnPressed);
        }
    }

    void ClampBGMAndSFXToMaster()
    {
        if (MasterVolume_Slider == null) return;

        float master = MasterVolume_Slider.value;

        if (BGM_Slider != null && BGM_Slider.value > master)
            BGM_Slider.value = master;

        if (SFX_Slider != null && SFX_Slider.value > master)
            SFX_Slider.value = master;
    }

    void UpdateVolume(TextMeshProUGUI labelText, float value, string key)
    {
        if (labelText != null)
            labelText.text = value.ToString("0") + "%";

        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();

        if (AudioSettings.Instance != null)
            AudioSettings.Instance.SetVolume(key, value);
    }

    float PercentToDB(float percent)
    {
        if (percent <= 0f) return -80f;
        float normalized = percent / 100f;

        if (normalized <= 1f)
        {
            return Mathf.Lerp(-40f, 0f, Mathf.Log10(1 + 9 * normalized));
        }

        return Mathf.Lerp(0f, 6f, normalized - 1f);
    }
}