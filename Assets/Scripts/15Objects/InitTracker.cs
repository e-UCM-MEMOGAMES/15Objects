using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Xasu;

public class InitTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private async void Init()
    {
        await XasuTracker.Instance.Init();
        await Task.Yield();
        while (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
        {
            await Task.Yield();
        }
    }

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
