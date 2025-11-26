using System;
using System.Threading.Tasks;
using UnityEngine;
using Xasu;
using Xasu.Config;

public class TrackerManager : SingletonMonoBehaviour<TrackerManager>
{
    public void InitTracker()
    {
        InitTrackerAsync();
    }

    /// <summary>
    /// Inicia el tracker
    /// </summary> 
    private async void InitTrackerAsync()
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

    private void OnApplicationQuit()
    {
        Debug.Log("Quitting");
        Quit();
    }

    public void Quit()
    {
        QuitAsync();
    }

    /// <summary>
    /// Cierra el tracker cuando se cierra el juego
    /// </summary> 
    private async Task QuitAsync()
    {
        if (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
            return;

        var progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {
            Debug.Log("Finalization progress: " + p);
        };
        await XasuTracker.Instance.Finalize(progress);
        Debug.Log("Tracker finalized");
    }
}
