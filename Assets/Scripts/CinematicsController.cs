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
    void Start()
    {
        currentImage = 0;
        showImage.sprite = cinematicsImages[currentImage];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextImage()
    {
       // if(currentImage < cinematicsImages.Length)
        {
            currentImage++;
           
           // showImage= cinematicsImages[currentImage];
           if (currentImage >= cinematicsImages.Length)
            {
                currentImage = 0;
                //showImage.sprite = cinematicsImages[currentImage];
            }
            showImage.sprite = cinematicsImages[currentImage];
        }
    }
}
