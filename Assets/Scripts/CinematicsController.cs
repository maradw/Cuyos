using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CinematicsController : MonoBehaviour
{
    [SerializeField] Sprite[] cinematicsImages;
    int currentImage ;
    [SerializeField] Image showImage;
    [SerializeField] Button nextScene;
    [SerializeField] Button nextImage;
    void Start()
    {
        nextScene.gameObject.SetActive(false);
        nextImage.gameObject.SetActive(true);
        nextImage.onClick.AddListener((NextImage));
        nextScene.onClick.AddListener(() => TransitionManager.Instance.LoadSceneByName("Level1"));
        currentImage = 0;
        showImage.sprite = cinematicsImages[currentImage];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy()
    {
        nextScene.onClick.RemoveAllListeners();
        nextImage.onClick.RemoveAllListeners();
    }

    public void NextImage()
    {
       // if(currentImage < cinematicsImages.Length)
        {
            currentImage++;
           
           // showImage= cinematicsImages[currentImage];
           if (currentImage +1 >= cinematicsImages.Length)
            {
                nextScene.gameObject.SetActive(true);
                nextImage.gameObject.SetActive(false);
                //currentImage = 0;
                //showImage.sprite = cinematicsImages[currentImage];
            }
            showImage.sprite = cinematicsImages[currentImage];
        }
    }
    
}
