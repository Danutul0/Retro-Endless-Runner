using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string audioParameter;
    [SerializeField] private float multiplier = 25f;


    public void SetupSlider()
    {
        volumeSlider.onValueChanged.AddListener(SliderValue);
        volumeSlider.minValue = .001f;
        volumeSlider.value = PlayerPrefs.GetFloat(audioParameter, volumeSlider.value);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(audioParameter, volumeSlider.value);
    }
    private void SliderValue(float value)
    {
        audioMixer.SetFloat(audioParameter, Mathf.Log10(value) * multiplier);
    }
}
