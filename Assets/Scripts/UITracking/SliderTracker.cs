using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xasu.HighLevel;

public class SliderTracker : MonoBehaviour
{
    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    TrackerManager trackerManager;

    /// <summary>
    /// Nombre del slider trackeado
    /// </summary>
    [SerializeField]
    string sliderName,
    /// <summary>
    /// Nombre del valor del slider que se esta trackeando
    /// </summary>
    valueExtension;

    /// <summary>
    /// Componente Slider del que obtener los valores
    /// </summary>
    Slider slider;


    void Start()
    {
        trackerManager = TrackerManager.Instance;
        slider = GetComponent<Slider>();
    }

    /// <summary>
    /// Llamado tanto al dejar de arrastrar como al cambiar el valor pulsando directamente
    /// </summary>
    public void PointerUp()
    {
        try
        {
            trackerManager.TrySendStatement(
                GameObjectTracker.Instance.Interacted(sliderName)
                .WithResultExtensions(new Dictionary<string, object> {
                { $"https://{valueExtension}", slider.value }
                })
            );
        }
        catch { }
    }
}
