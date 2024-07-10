using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTracker : MonoBehaviour
{
    Slider slider;
    [SerializeField]
    string sliderName;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(Interacted);

        slider.interactable = true;
    }

    public async void Interacted(float value)
    {
        slider.interactable = false;
        await Xasu.HighLevel.GameObjectTracker.Instance.Interacted(sliderName);
        slider.interactable = true;
    }

}
