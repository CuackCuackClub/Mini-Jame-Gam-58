using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_AudioOptions : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        BindSlider(musicSlider, GetMusicVolume(), SetMusicVolume);
        BindSlider(sfxSlider, MusicManager.MasterSFXVolume, SetSfxVolume);
    }

    private void OnDisable()
    {
        UnbindSlider(musicSlider, SetMusicVolume);
        UnbindSlider(sfxSlider, SetSfxVolume);
    }

    private static float GetMusicVolume()
    {
        return MusicManager.Instance != null ? MusicManager.Instance.MasterMusicVolume : 1f;
    }

    private static void SetMusicVolume(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMusicVolume(value);
        }
    }

    private static void SetSfxVolume(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetSFXVolume(value);
        }
    }

    private static void BindSlider(Slider slider, float value, UnityAction<float> handler)
    {
        if (slider == null)
        {
            return;
        }

        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(handler);
    }

    private static void UnbindSlider(Slider slider, UnityAction<float> handler)
    {
        if (slider == null)
        {
            return;
        }

        slider.onValueChanged.RemoveListener(handler);
    }
}
