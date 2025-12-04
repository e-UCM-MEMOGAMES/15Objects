using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Xasu;
using Xasu.Config;
using Xasu.HighLevel;

public class TrackerManager : SingletonMonoBehaviour<TrackerManager>
{
    [SerializeField]
    bool defaultOnlineMode = true;

    [SerializeField]
    string defaultLRSEndpoint = "https://my.lrs.endpoint/",
           defaultAuthProtocol = "basic",
           defaultUsername = "your-username",
           defaultPassword = "your-password";


    private void Start()
    {
        InitTrackerAsync();
    }

    /// <summary>
    /// Inicia el tracker
    /// </summary> 
    private async void InitTrackerAsync()
    {
        if (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
        {
            if (File.Exists(Path.Combine("StreamingAssets", "tracker_config.json")))
            {
                await XasuTracker.Instance.Init();
            }
            else
            {
                await XasuTracker.Instance.Init(new TrackerConfig
                {
                    Online = defaultOnlineMode,
                    LRSEndpoint = defaultLRSEndpoint,
                    AuthProtocol = defaultAuthProtocol,
                    AuthParameters = new Dictionary<string, string> {
                        { "username", defaultUsername },
                        { "password", defaultPassword }
                    }
                });
            }
        }
    }


    /// <summary>
    /// Cierra el tracker cuando se cierra el juego
    /// </summary> 
    public async Task Quit()
    {
        if (XasuTracker.Instance.Status.State == TrackerState.Uninitialized || XasuTracker.Instance.Status.State ==  TrackerState.Finalized)
        {
            return;
        }
        Debug.Log("Quitting TrackerManager");

        Progress<float> progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {
            Debug.Log("Finalization progress: " + p);
        };
        await XasuTracker.Instance.Finalize(progress);
        Debug.Log("Tracker finalized");
    }


    public async void TrySendStatement(StatementPromise promise)
    {
        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        {
            try
            {
                var statement = await promise.Promise;
                Debug.Log("Completed statement sent with id: " + promise.Statement.id);
            }
            catch (AggregateException aggEx)
            {
                Debug.Log("Failed! " + aggEx.GetType().ToString());
                foreach (var ex in aggEx.InnerExceptions)
                {
                    Debug.Log("Inner Exception: " + ex.GetType().ToString());
                }
            }
        }
    }

}
