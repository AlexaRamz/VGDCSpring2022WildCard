using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite greyHeart;
    private bool isGrey;
    private Image h_image;


    // Start is called before the first frame update
    void Start()
    {
        isGrey = false;
        h_image = gameObject.GetComponent<Image>();


    }

    public void TurnHeartGrey()
    {
        h_image.sprite = greyHeart;
        isGrey = true;
    }

    public void TurnHeartRed()
    {
        h_image.sprite = fullHeart;
        isGrey = false;
    }
}
