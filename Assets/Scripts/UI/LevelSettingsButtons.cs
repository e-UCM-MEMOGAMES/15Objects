using UnityEngine;
using Xasu;
using Xasu.HighLevel;

public class LevelSettingsButtons : MonoBehaviour
{
    [SerializeField]
    GameObject gameModeSelector, levelSelector;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        gameModeSelector.SetActive(true);
        levelSelector.SetActive(false);
    }

    public void Return()
    {
        if (gameModeSelector.activeSelf)
        {
            gameManager.ChangeScene(gameManager.MENU_SCENE);
        }
        else if (levelSelector.activeSelf)
        {
            gameModeSelector.SetActive(true);
            levelSelector.SetActive(false);
        }
    }

    public void SelectGamemode(GameObject elements)
    {
        gameModeSelector.SetActive(false);
        levelSelector.SetActive(true);

        gameManager.GamemodeElements = elements;
    }

    public void StartTutorial()
    {
        gameManager.ChangeScene(gameManager.TUTORIAL_SCENE);
    }

    public void SelectLevelItems(GameObject items)
    {
        gameManager.LevelItems = items;
    }
    public void StartGame()
    {
        gameManager.ChangeScene(gameManager.GAME_SCENE);

        // TODO
        //if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        //    CompletableTracker.Instance.Initialized(level, CompletableTracker.CompletableType.Level);
    }
}
