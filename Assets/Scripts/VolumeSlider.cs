using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] private VolumeType volumeType;
    [SerializeField] private AudioData audioData;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        switch (volumeType)
        {
            case VolumeType.Master:
                slider.value = audioData._master;
                break;

            case VolumeType.Music:
                slider.value = audioData._music;
                break;

            case VolumeType.SFX:
                slider.value = audioData._SFX;
                break;
        }
        slider.onValueChanged.AddListener(OnSliderChanged);
    }
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                MusicManager.Instance.SetMaster(value);

                break;

            case VolumeType.Music:
                MusicManager.Instance.SetMusic(value);
                break;

            case VolumeType.SFX:
                MusicManager.Instance.SetSFX(value);
                break;
        }
    }
}
