using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        slider.interactable = false;
        await Xasu.HighLevel.GameObjectTracker.Instance.Interacted(sliderName);
        slider.interactable = true;
    }

}
