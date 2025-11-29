using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xasu;
using Xasu.HighLevel;

public class InteractedTracker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
                Collider2D[] result = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                int i = result.Length;
                if (i == 0)
                {
                    Dictionary<string, object> extensions = new Dictionary<string, object> {
                        { Application.identifier + "://" + "empty", 1 }
                    };
                    AlternativeTracker.Instance.Selected("Pointer", "empty").WithResultExtensions(extensions);
                }
                else
                {
                    foreach (Collider2D item in result)
                    {
                        if (item.name != null)
                        {
                            GameObjectTracker.Instance.Interacted(item.name);
                        }
                    }
                }

                //Return the current Active Scene in order to get the current Scene's name
                string sceneName = SceneManager.GetActiveScene().name;

                if (sceneName == "15Objects")
                {
                    GameObject obj = GameObject.Find("A");
                    if (obj != null)
                    {
                        sceneName += "-A";
                    }
                    else
                    {
                        sceneName += "-B";
                    }
                    GameObjectTracker.Instance.Interacted(sceneName);
                }
            }
        }
    }
}
