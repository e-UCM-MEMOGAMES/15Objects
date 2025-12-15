using UnityEngine;


public class LevelSettingsButtons : MonoBehaviour
{
    /// <summary>
    /// Instancia del GameManager
    /// </summary>
    GameManager gameManager;

    /// <summary>
    /// Objeto con todos los elementos de la pantalla de seleccion de modo de juego
    /// </summary>
    [SerializeField]
    GameObject gameModeSelector,
    /// <summary>
    /// Objeto con todos los elementos de la pantalla de seleccion de modo de nivel
    /// </summary>
    levelSelector;


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
            gameManager.ChangeScene(Defs.MENU_SCENE_NAME);
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

    public void SelectLevelItems(GameObject items)
    {
        gameManager.LevelItems = items;
    }
    public void StartGame()
    {
        gameManager.ChangeScene(Defs.GAME_SCENE_NAME);
    }
}
