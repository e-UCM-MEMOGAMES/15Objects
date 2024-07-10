using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundComponent : MonoBehaviour
{
    [SerializeField]
    GameSound gameSound;
    AudioManager audioMng;
  
    // Start is called before the first frame update
    void Start()
    {
        audioMng = AudioManager.Instance;
    }
    public void PlaySound()
    {
        audioMng.Play(gameSound);
    }


}
