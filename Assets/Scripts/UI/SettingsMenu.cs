using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Xasu.HighLevel;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

    /// <summary>
    /// Instancia del AudioManager
    /// </summary>
    AudioManager audioManager;


    /// <summary>
    /// Slider de volumen de la musica de fondo
    /// </summary>
    [SerializeField]
    Slider bgmSlider,

    /// <summary>
    /// Slider de volumen de los efectos de sonido
    /// </summary>
    sfxSlider;


    /// <summary>
    /// Dropdown con los idiomas disponibles
    /// </summary>
    [SerializeField]
    TMP_Dropdown dropdown;

    /// <summary>
    /// Lista con las localizaciones disponibles
    /// </summary>
    List<Locale> lcs;

   

    void Awake()
    {
        trackerManager = TrackerManager.Instance;
        lcs = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < lcs.Count; ++i)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = lcs[i].LocaleName });
        }

        if (PlayerPrefs.HasKey("language"))
        {
            dropdown.value = PlayerPrefs.GetInt("language");
        }
        else
        {
            int lid = lcs.IndexOf(LocalizationSettings.SelectedLocale);
            dropdown.value = lid;
            PlayerPrefs.SetInt("language", lid);
        }
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        bgmSlider.value = audioManager.GetBGMVolume();
        sfxSlider.value = audioManager.GetSoundEffectVolume();
    }

    public void ChangeLanguage(TMP_Dropdown dropDown)
    {
        LocalizationSettings.SelectedLocale = lcs[dropDown.value];
        PlayerPrefs.SetInt("language", dropDown.value);

        trackerManager.TrySendStatement(AlternativeTracker.Instance.Selected(lcs[dropDown.value].LocaleName, "Language"));
    }

    public void SetBGMVolume(float volume)
    {
        audioManager.BGMVolume(volume);
    }

    public void setSFXVolume(float volume) 
    {
        audioManager.SoundEffectVolume(volume);
    }
}
