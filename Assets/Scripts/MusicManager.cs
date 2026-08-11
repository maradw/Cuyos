using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioGameMixer;
    [SerializeField] private AudioData _volumeData;
    [SerializeField] private AudioSource _buttonSound;
    public static MusicManager Instance;
    [SerializeField] private AudioSource audioSource;
    public MusicData CurrentMusic { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {


        SetMaster(_volumeData._master);
        SetMusic(_volumeData._music);
        SetSFX(_volumeData._SFX);
    }
    public void SetMaster(float f)
    {
        _volumeData._master = f;
        _audioGameMixer.SetFloat("Master", Mathf.Log10(f) * 20f);
    }
    public void SetMusic(float f)
    {
        _volumeData._music = f;
        _audioGameMixer.SetFloat("Music", Mathf.Log10(f) * 20f);
    }
    public void SetSFX(float f)
    {
        _volumeData._SFX = f;
        _audioGameMixer.SetFloat("SFX", Mathf.Log10(f) * 20f);
    }
    public void PlayBG(MusicData musicData)
    {
        StartCoroutine(PlayBGCoroutine(musicData));
    }

    private IEnumerator PlayBGCoroutine(MusicData musicData)
    {
        if (CurrentMusic == musicData)
            yield break;

        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutCoroutine(1f));
        }

        CurrentMusic = musicData;

        audioSource.clip = musicData.Clip;
        audioSource.loop = true;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    public void StopFade(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
       
        
        //audioSource.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
