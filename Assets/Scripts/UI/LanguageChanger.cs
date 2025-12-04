using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    List<Locale> lcs;

    [SerializeField]
    TMP_Dropdown dropdown;

    void Awake()
    {
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

    public void OnDropDownChanged(TMP_Dropdown dropDown)
    {
        LocalizationSettings.SelectedLocale = lcs[dropDown.value];
        PlayerPrefs.SetInt("language", dropDown.value);

        // TODO: Que tipo de traza usar?
    }
}
