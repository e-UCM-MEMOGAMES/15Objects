using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Xasu;
using Xasu.Config;
using Xasu.HighLevel;

public class TrackerManager : SingletonMonoBehaviour<TrackerManager>
{
    /// <summary>
    /// Instancia del tracker
    /// </summary> 
    XasuTracker tracker;

    Task initTask = null;
    public Task InitTask {  get { return initTask; } }

    void Start()
    {
        tracker = XasuTracker.Instance;
        initTask = InitTrackerAsync();
    }

    /// <summary>
    /// Inicia el tracker
    /// </summary> 
    private async Task InitTrackerAsync()
    {
        if (tracker.Status.State == TrackerState.Uninitialized)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "tracker_config.json");
            bool configExists = File.Exists(filePath);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                UnityWebRequest request = UnityWebRequest.Get(filePath);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    configExists = true;
                }
            }

            // Si hay un archivo de configuracion, se inicializa con esa configuracion
            if (configExists)
            {
                await tracker.Init();
            }
            // Si no, se inicializa por defecto para que se guarde en local
            else
            {
                await tracker.Init(new TrackerConfig
                {
                    Offline = true,
                    TraceFormat = TraceFormats.XAPI
                }, null);
            }
        }
    }


    /// <summary>
    /// Cierra el tracker cuando se cierra el juego
    /// </summary> 
    public async Task Quit()
    {
        // Si el tracker no se ha inicializado o si ha finalizado, no hace nada
        if (tracker.Status.State == TrackerState.Uninitialized || tracker.Status.State == TrackerState.Finalized)
        {
            return;
        }
        Debug.Log("Quitting TrackerManager");

        Progress<float> progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {
            Debug.Log("Finalization progress: " + p);
        };
        await tracker.Finalize(progress);
        Debug.Log("Tracker finalized");
    }


    public async void TrySendStatement(StatementPromise promise)
    {
        // Si el tracker esta en estado normal, se intenta enviar la traza
        if (tracker.Status.State == TrackerState.Normal)
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
