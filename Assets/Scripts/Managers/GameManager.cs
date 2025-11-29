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
    public SceneAsset MENU_SCENE, SETTINGS_SCENE, CREDITS_SCENE, LEVEL_SETTINGS_SCENE, GAME_SCENE, TUTORIAL_SCENE;


    TrackerManager trackerManager;

    [SerializeField]
    GameObject gamemodeElements;
    public GameObject GamemodeElements
    {
        get { return gamemodeElements; }
        set { gamemodeElements = value; }
    }

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
    public void ChangeScene(SceneAsset scene)
    {
        ChangeScene(scene.name);
    }
    public void ChangeScene(string sceneName)
    {
        //if (/*SceneManager.GetActiveScene().name != "Start" && */XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        //{
        //    AccessibleTracker.Instance.Accessed(sceneName, AccessibleTracker.AccessibleType.Screen);
        //}
        SceneManager.LoadScene(sceneName);
    }


}
