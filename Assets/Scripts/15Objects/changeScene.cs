using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xasu;
using Xasu.HighLevel;

public class changeScene : MonoBehaviour {
    public void ChangeScene(string scene)
    {
        if (SceneManager.GetActiveScene().name != "Start" && XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
            AccessibleTracker.Instance.Accessed(scene, AccessibleTracker.AccessibleType.Screen);
        if (scene != "exit")
            SceneManager.LoadScene(scene);
        else
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

}
