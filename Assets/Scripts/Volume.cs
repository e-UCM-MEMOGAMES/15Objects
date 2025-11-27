using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField]
    ChannelType channelType;
    AudioManager audioMng;
    Slider slider;
    void Start()
    {
        audioMng = AudioManager.Instance;
        slider = GetComponent<Slider>();
        if (channelType == ChannelType.BGM)
            slider.value = audioMng.GetBGMVolume();
        else slider.value = audioMng.GetSoundEffectVolume();

    }
    public void SetVolume(float volume)
    {
        if (channelType == ChannelType.BGM)
            audioMng.BGMVolume(volume);
        else audioMng.SoundEffectVolume(volume);

    }
}
