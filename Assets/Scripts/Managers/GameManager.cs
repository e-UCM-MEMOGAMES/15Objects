using System.Diagnostics;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Xasu.HighLevel;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// Temporizador para medir el tiempo que se tarda en completar el nivel
    /// </summary>
    Stopwatch watch = Stopwatch.StartNew();

    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

    string COMPLETABLE_ID = "game";
    CompletableTracker.CompletableType COMPLETABLE_TYPE = CompletableTracker.CompletableType.Game;


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


    //    Start is called before the first frame update
    void Start()
    {
        trackerManager = TrackerManager.Instance;
        trackerManager.TrySendStatement(CompletableTracker.Instance.Initialized(COMPLETABLE_ID, COMPLETABLE_TYPE));
        trackerManager.TrySendStatement(AccessibleTracker.Instance.Accessed(Defs.MENU_SCENE_NAME));

        watch.Start();

        if (PlayerPrefs.HasKey(Defs.LANGUAGE_KEY))
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt(Defs.LANGUAGE_KEY)];
        }

        GamemodeElements = gamemodeElements;
        LevelItems = levelItems;

        //Application.wantsToQuit += WantsToQuit;
    }

    /// <summary>
    /// Llamado al generar el evento de que se quiere salir de la aplicacion
    /// (tanto al pulsar el boton de salir como al salir pulsando la X)
    /// </summary>
    async void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Quitting GameManager");
        watch.Stop();

        trackerManager.TrySendStatement(CompletableTracker.Instance.Completed(COMPLETABLE_ID, COMPLETABLE_TYPE, watch.ElapsedMilliseconds));
        await trackerManager.Quit();
    }

    /// <summary>
    /// Llamado al pulsar el boton de salir. Se encarga de cerrar el juego
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }


    /// <summary>
    /// Se cambia a la escena indicada por el parametro que se pasa
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        trackerManager.TrySendStatement(AccessibleTracker.Instance.Accessed(sceneName));
        SceneManager.LoadScene(sceneName);
    }
}
