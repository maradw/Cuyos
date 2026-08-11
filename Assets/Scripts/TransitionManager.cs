using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

   [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject audioSettingsPanel;

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
   
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #else
            Application.Quit();
        #endif
    }
    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        StartCoroutine(FadeIn());
        audioSettingsPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return FadeOut();
        

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return FadeIn();
       
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ShowAudioSettings()
    {
        audioSettingsPanel.SetActive(true);
    }
    public void LoadSceneCorrutine(string scenename)
    {
        StartCoroutine(nextScene(scenename));
    }

    private IEnumerator nextScene(string scenename)
    {
       yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scenename);
    }

    private IEnumerator FadeIn()
    {
        Color color = fadeImage.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
        color.a = 0;
        fadeImage.color = color;
       
    }

    private IEnumerator FadeOut()
    {
        Color color = fadeImage.color;
        fadeImage.raycastTarget = true;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
      
    }

}
    

