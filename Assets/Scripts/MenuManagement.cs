using Game.Audio;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagement : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button AudioSettings;
    [SerializeField] MusicData BgMusic;
    void Start()
    {
        MusicManager.Instance.PlayBG(BgMusic);
        //startButton.onClick.AddListener(() => MusicManager.Instance.StopFade(0.5f));
        startButton.onClick.AddListener(() => TransitionManager.Instance.LoadSceneCorrutine("CinematicaInicio"));
       exitButton.onClick.AddListener(TransitionManager.Instance.ExitGame);
        //creditsButton.onClick.AddListener(() => TransitionManager.Instance.LoadSceneByName("Credits"));
        AudioSettings.onClick.AddListener(TransitionManager.Instance.ShowAudioSettings);
        
    }

    void Update()
    {
        
    }
    void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        creditsButton.onClick.RemoveAllListeners();
        AudioSettings.onClick.RemoveAllListeners();
    }
   
}
