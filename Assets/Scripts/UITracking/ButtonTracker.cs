using UnityEngine;
using UnityEngine.UI;
using Xasu.HighLevel;

public class ButtonTracker : MonoBehaviour
{
    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

    /// <summary>
    /// Nombre del boton trackeado
    /// </summary>
    [SerializeField]
    string buttonName;


    // Start is called before the first frame update
    void Start()
    {
        trackerManager = TrackerManager.Instance;

        try
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.001f;
        }
        catch { }
    }

    public void Track()
    {
        trackerManager.TrySendStatement(GameObjectTracker.Instance.Interacted(buttonName));
    }
}
