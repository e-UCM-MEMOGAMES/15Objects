using UnityEngine;
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
    }

    public void Track()
    {
        trackerManager.TrySendStatement(GameObjectTracker.Instance.Interacted(buttonName));
    }
}
