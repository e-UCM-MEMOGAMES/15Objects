using UnityEngine;
using UnityEngine.UI;
using Xasu;

public class SliderTracker : MonoBehaviour
{
    Slider slider;

    [SerializeField]
    string sliderName;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(Interacted);
        slider.interactable = true;
    }

    /// <summary>
    /// Se llama cuando se ha interactuado con el slider
    /// </summary> 
    public async void Interacted(float value)
    {
        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        {
            return;
        }

        slider.interactable = false;
        await Xasu.HighLevel.GameObjectTracker.Instance.Interacted(sliderName);
        slider.interactable = true;
    }

}
