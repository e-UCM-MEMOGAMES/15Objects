using UnityEngine;
using Xasu.HighLevel;

public class ButtonTracker : MonoBehaviour
{
    [SerializeField]
    string buttonName;

    TrackerManager trackerManager;

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
