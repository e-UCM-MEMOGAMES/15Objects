using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Xasu;
using Xasu.Config;

public class InitTracker : MonoBehaviour
{
    public void InitTrack()
    {
        Init();
    }

    /// <summary>
    /// Inicia el tracker
    /// </summary> 
    private async void Init()
    {
        bool hasConfig = false;
        try
        {
            var trackerConfig = await TrackerConfigLoader.LoadLocalAsync();
            hasConfig = true;
        }
        catch { Debug.Log("Tracker config not found."); }
        if (hasConfig)
        {
            await XasuTracker.Instance.Init();
            await Task.Yield();
            while (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
            {
                await Task.Yield();
            }
        }
    }

    /// <summary>
    /// Cierra el tracker cuando se cierra el juego
    /// </summary> 
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
