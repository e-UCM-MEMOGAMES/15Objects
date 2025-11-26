using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameState15O : SingletonMonoBehaviour<GameState15O>
{
    private bool _fileConfig;
    public bool fileConfig
    {
        get { return _fileConfig; }
        set { _fileConfig = value; }
    }

    private void Awake()
    {
        base.Awake();
        _fileConfig = false;
    }

    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.Play(GameSound.MenuBGM);

        if (PlayerPrefs.HasKey("language"))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("language")];
        }
    }
}
