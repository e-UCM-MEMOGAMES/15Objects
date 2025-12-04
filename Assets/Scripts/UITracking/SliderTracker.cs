using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xasu.HighLevel;

public class SliderTracker : MonoBehaviour
{
    TrackerManager trackerManager;

    [SerializeField]
    string sliderName;
    Slider slider;


    void Start()
    {
        trackerManager = TrackerManager.Instance;
        slider = GetComponent<Slider>();
    }

    public void Track()
    {
        trackerManager.TrySendStatement(
            GameObjectTracker.Instance.Interacted(sliderName)
            .WithResultExtensions(new Dictionary<string, object> {
                { "https://" + "volume", slider.value }
            })
        );
    }
}
