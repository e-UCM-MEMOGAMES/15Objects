using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtons : MonoBehaviour
{
    [SerializeField]
    GameObject gameModeSelector, levelSelector, 
               selectorGameMode, writeGameMode, levelsObj;

    Level levelManager;


    // Start is called before the first frame update
    void Start()
    {
        gameModeSelector.SetActive(true);
        levelSelector.SetActive(false);
        levelsObj.SetActive(false);

        levelManager = levelsObj.GetComponent<Level>();
    }

    public void Return()
    {
        if (gameModeSelector.activeSelf)
        {
            SceneManager.LoadScene("Start");
        }
        else if (levelSelector.activeSelf)
        {
            gameModeSelector.SetActive(true);
            levelSelector.SetActive(false);
            levelsObj.SetActive(false);
        }
        else
        {
            if (levelsObj.activeSelf)
            {
                levelManager.Return();
            }
            else
            {
                gameModeSelector.SetActive(false);
                levelSelector.SetActive(true);
                levelsObj.SetActive(false);
            }
        }
    }

    public void SelectGamemode()
    {
        gameModeSelector.SetActive(false);
        levelSelector.SetActive(true);
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial15O");
    }

    public void SelectLevel()
    {
        levelSelector.SetActive(false);
        levelsObj.SetActive(true);
    }
}
