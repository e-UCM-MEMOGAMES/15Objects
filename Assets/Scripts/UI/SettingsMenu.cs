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
    /// Instancia del GameManager
    /// </summary>
    GameManager gameManager;
    /// <summary>
    /// Instancia del AudioManager
    /// </summary>
    AudioManager audioManager;
    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

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


    private void Start()
    {
        audioManager = AudioManager.Instance;
        bgmSlider.value = audioManager.BGMVolume;
        sfxSlider.value = audioManager.SFXVolume;

        trackerManager = TrackerManager.Instance;
        lcs = LocalizationSettings.AvailableLocales.Locales;

        // Se rellena el dropdown de idiomas con las opciones disponibles
        for (int i = 0; i < lcs.Count; ++i)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = lcs[i].LocaleName });
        }

        int lid = lcs.IndexOf(LocalizationSettings.SelectedLocale);

        // Si hay un idioma guardado, se usa su id
        if (PlayerPrefs.HasKey(Defs.LANGUAGE_PREFS_KEY))
        {
            lid = PlayerPrefs.GetInt(Defs.LANGUAGE_PREFS_KEY);
        }
        // Si no, se guarda la id del idioma elegido previamente
        else
        {
            PlayerPrefs.SetInt(Defs.LANGUAGE_PREFS_KEY, lid);
        }

        // Se cambia el valor del slider sin llamar al OnValueChanged y se fuerza la actualizacion de su valor
        dropdown.SetValueWithoutNotify(lid);
        dropdown.RefreshShownValue();
    }

    /// <summary>
    /// Llamado al cambiar el valor del dropdown del idioma
    /// </summary>
    public void ChangeLanguage()
    {
        LocalizationSettings.SelectedLocale = lcs[dropdown.value];
        PlayerPrefs.SetInt(Defs.LANGUAGE_PREFS_KEY, dropdown.value);

        trackerManager.TrySendStatement(AlternativeTracker.Instance.Selected("Language", lcs[dropdown.value].LocaleName));
    }

    /// <summary>
    /// Llamado al cambiar el valor del slider de la musica
    /// </summary>
    public void SetBGMVolume()
    {
        audioManager.BGMVolume = bgmSlider.value;
    }
    /// <summary>
    /// Llamado al cambiar el valor del slider de los efectos de sonido
    /// </summary>
    public void SetSFXVolume()
    {
        audioManager.SFXVolume = sfxSlider.value;
    }
}
