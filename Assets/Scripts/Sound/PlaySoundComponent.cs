using UnityEngine;

public class PlaySoundComponent : MonoBehaviour
{
    /// <summary>
    /// Sonido que se reproduce
    /// </summary> 
    [SerializeField]
    GameSound gameSound;
    /// <summary>
    /// Manager de sonidos
    /// </summary> 
    AudioManager audioMng;
  
    // Start is called before the first frame update
    void Start()
    {
        audioMng = AudioManager.Instance;
    }
    /// <summary>
    /// Reproduce el sonido
    /// </summary> 
    public void PlaySound()
    {
        audioMng.Play(gameSound);
    }
}
