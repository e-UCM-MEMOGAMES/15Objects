using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameM : SingletonMonoBehaviour<GameM>
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("language"))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("language")];
        }
    }
}
