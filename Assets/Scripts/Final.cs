using UnityEngine;
using UnityEngine.UI;
public class Final : MonoBehaviour
{
    [SerializeField] Button exitButton;
    void Start()
    {
        exitButton.onClick.AddListener(() => TransitionManager.Instance.LoadSceneCorrutine("Menu"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }
}
