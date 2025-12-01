using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Xasu;
using Xasu.HighLevel;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// Nomnbre de la escena del menu principal
    /// </summary>
    public string MENU_SCENE_NAME = "Start",
    /// <summary>
    /// Nomnbre de la escena de configuracion
    /// </summary>
    SETTINGS_SCENE_NAME = "Settings",
    /// <summary>
    /// Nomnbre de la escena de creditos
    /// </summary>
    CREDITS_SCENE_NAME = "Credits",
    /// <summary>
    /// Nomnbre de la escena de opciones del nivel
    /// </summary>
    LEVEL_SETTINGS_SCENE_NAME = "LevelSettings",
    /// <summary>
    /// Nomnbre de la escena de juego
    /// </summary>
    GAME_SCENE_NAME = "Game";

    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

    /// <summary>
    /// Prefab con los elementos del modo de juego seleccionado
    /// </summary>
    [SerializeField]
    GameObject gamemodeElements;
    public GameObject GamemodeElements
    {
        get { return gamemodeElements; }
        set { gamemodeElements = value; }
    }

    /// <summary>
    /// Prefab con los objetos del nivel seleccionado
    /// </summary>
    [SerializeField]
    GameObject levelItems;
    public GameObject LevelItems
    {
        get { return levelItems; }
        set { levelItems = value; }
    }


    bool fileConfig = false;
    FileStream fs;


    //    Start is called before the first frame update
    void Start()
    {
        trackerManager = TrackerManager.Instance;

        if (PlayerPrefs.HasKey("language"))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("language")];
        }

        GamemodeElements = gamemodeElements;
        LevelItems = levelItems;
    }

    /// <summary>
    /// Llamado al pulsar el boton de salir. Se encarga de cerrar el juego
    /// </summary>
    public void ExitGame()
    {
        ExitGameAsync();
    }
    private async Task ExitGameAsync()
    {
        await trackerManager.QuitAsync();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        Debug.Log("Game closed");
#else
    		Application.Quit();
#endif
    }


    /// <summary>
    /// Se cambia a la escena indicada por el parametro que se pasa
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        //if (/*SceneManager.GetActiveScene().name != "Start" && */XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        //{
        //    AccessibleTracker.Instance.Accessed(sceneName, AccessibleTracker.AccessibleType.Screen);
        //}
        SceneManager.LoadScene(sceneName);
    }


}
