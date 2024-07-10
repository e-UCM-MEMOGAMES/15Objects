using System.Collections;
using System.Collections.Generic;
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
    TMPro.TMP_Dropdown dropdown;


    // Start is called before the first frame update
    void Start()
    {
        lcs = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < lcs.Count; ++i)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = lcs[i].LocaleName });
        }

        Debug.Log(lcs.IndexOf(LocalizationSettings.SelectedLocale));

        dropdown.value = -1;
        dropdown.value = lcs.IndexOf(LocalizationSettings.SelectedLocale);

    }

    public void OnDropDownChanged(TMP_Dropdown dropDown)
    {
        Debug.Log("DROP DOWN CHANGED -> " + dropDown.value);
        LocalizationSettings.SelectedLocale = lcs[dropDown.value];
    }


  
}
