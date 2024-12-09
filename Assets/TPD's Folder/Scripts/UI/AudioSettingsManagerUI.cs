using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsManagerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider masterSlider; // Slider cho Master
    public Slider musicSlider;  // Slider cho Music
    public Slider sfxSlider;    // Slider cho SFX

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Gắn Audio Mixer tại đây

    private static AudioSettingsManagerUI instance; // Singleton instance

    void Awake()
    {
        // Singleton pattern để script chỉ tồn tại một lần
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không phá hủy khi load Scene mới
        }
        else
        {
            Destroy(gameObject); // Xóa các bản sao thừa
            return;
        }
    }

    void Start()
    {
        // Gán giá trị ban đầu cho các slider từ PlayerPrefs
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // Cập nhật âm lượng ban đầu
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        // Đăng ký sự kiện khi slider thay đổi
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        if (value == 0)
        {
            audioMixer.SetFloat("MasterVolume", -80f); // Tắt âm thanh hoàn toàn
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        if (value == 0)
        {
            audioMixer.SetFloat("MusicVolume", -80f); // Tắt âm thanh hoàn toàn
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value == 0)
        {
            audioMixer.SetFloat("SFXVolume", -80f); // Tắt âm thanh hoàn toàn
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    void OnDestroy()
    {
        // Lưu giá trị khi GameObject bị phá hủy
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }
}
