using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xasu;
using Xasu.HighLevel;   

public class InteractedTracker : MonoBehaviour {
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            //Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
            Collider2D[] result = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            int i = result.Length;
            if (i == 0)
            {
                AlternativeTracker.Instance.Selected("Pointer", "empty");
                //Tracker.T.setVar("empty", 1);
            }
            else
            {
                foreach (Collider2D item in result)
                    if (item.name != null) GameObjectTracker.Instance.Interacted(item.name);
            }

            //Return the current Active Scene in order to get the current Scene's name
            Scene scene = SceneManager.GetActiveScene();
            string name = scene.name;


            if (scene.name == "15Objects")
            {
                GameObject a = GameObject.Find("A");
                if (a != null)
                {
                    name += "-A";
                }
                else
                {
                    name += "-B";
                }
            }
            GameObjectTracker.Instance.Interacted(name);
            //Tracker.T.GameObject.Interacted(name);
        }
    }

    private async Task OnApplicationQuitAsync()
    {
        var progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {
            Debug.Log("Finalization progress: " + p);
        };
        await XasuTracker.Instance.Finalize(progress);
        Debug.Log("Tracker finalized");
        Application.Quit();
    }
}
