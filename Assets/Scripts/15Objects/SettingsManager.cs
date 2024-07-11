using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    List<Locale> lcs;
    [SerializeField]
    TMP_Dropdown dropdown;


    // Start is called before the first frame update
    void Start()
    {
        lcs = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < lcs.Count; ++i)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = lcs[i].LocaleName });
        }

        if (PlayerPrefs.HasKey("language"))
            dropdown.value = PlayerPrefs.GetInt("language");
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
    }
}
