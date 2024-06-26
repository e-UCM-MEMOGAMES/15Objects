using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xasu.HighLevel;

public class changeScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScene(string scene)
    {
		AccessibleTracker.Instance.Accessed(scene, AccessibleTracker.AccessibleType.Screen);
        //Tracker.T.Accessible.Accessed(scene, AccessibleTracker.Accessible.Screen);
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
