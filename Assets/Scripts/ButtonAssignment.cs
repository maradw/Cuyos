using UnityEngine;
using UnityEngine.UI;

public class ButtonAssignment : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button AudioSettings;
    void Start()
    {
       startButton.onClick.AddListener(() => TransitionManager.Instance.LoadSceneByName("CinematicaInicio"));
       exitButton.onClick.AddListener(TransitionManager.Instance.ExitGame);
        //creditsButton.onClick.AddListener(() => TransitionManager.Instance.LoadSceneByName("Credits"));
        AudioSettings.onClick.AddListener(TransitionManager.Instance.ShowAudioSettings);
    }

    // Update is called once per frame
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
