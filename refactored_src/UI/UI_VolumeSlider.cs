using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour, ISaveManager
{
    public Slider slider;
    public string parameters;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;

    public void LoadData(GameData data)
    {
        if (parameters == "sfx")
            slider.value = data.SFXVolume;
        if (parameters == "bgm")
            slider.value = data.BGMVolume;
            
        // 加载后立即更新音量
        SliderValue(slider.value);
    }

    public void SaveData(ref GameData data)
    {
        if (parameters == "sfx")
            data.SFXVolume = slider.value;
        if (parameters == "bgm")
            data.BGMVolume = slider.value;
    }

    public void SliderValue(float value)
    {
        // 0 dB 是参考点，负值表示相对于最大音量的衰减程度
        audioMixer.SetFloat(parameters, Mathf.Log10(value) * multiplier);
    }
}
