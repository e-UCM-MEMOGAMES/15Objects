using UnityEditor;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    GameManager gameManager;

    //Staart is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void ChangeScene(SceneAsset scene)
    {
        ChangeScene(scene.name);
    }
    public void ChangeScene(string sceneName)
    {
        gameManager.ChangeScene(sceneName);
    }

    public void ExitGame()
    {
        gameManager.ExitGame();
    }
}
