using UnityEngine;
using UnityEngine.SceneManagement;
using Xasu;
using Xasu.HighLevel;

public class MenuButtons : MonoBehaviour {
    /// <summary>
    /// Se cambia a la escena indicada por el parametro que se pasa
    /// </summary> 
    public void ChangeScene(string scene)
    {
        if (SceneManager.GetActiveScene().name != "Start" && XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        {
            AccessibleTracker.Instance.Accessed(scene, AccessibleTracker.AccessibleType.Screen);
        }
        SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        Debug.Log("Game closed");
#else
		Application.Quit();
#endif
    }
}
