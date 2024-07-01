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
        Initialize();

    }
    async void Initialize()
    {
        await XasuTracker.Instance.Init();
        await Task.Yield();

        while (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
        {
            await Task.Yield();
        }
    }
}
