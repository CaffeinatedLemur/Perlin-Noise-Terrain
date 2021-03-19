using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/************************************
 * Input handler script
 * Written by me. Takes the values from the inputs of the UI from the user and applys them the the values of the varaibles in GameManager
 * **********************************/
public class InputHandler : MonoBehaviour
{
    //Each input field in the UI
    [SerializeField] public Slider scaleSlider;
    [SerializeField] public Slider detailSlider;
    [SerializeField] public Slider ecenSlider;
    [SerializeField] public InputField seedIF;
    public void GetData()
    {
        //get the seed if user added one, otherwise generate a random seed
        try
        {
            GameManager.seed = float.Parse(seedIF.text);
        }
        catch
        {
            GameManager.seed = Random.Range(0, 30000);
        }
        //set the other values to whatever value the slider is on.
        GameManager.scale = scaleSlider.value;
        GameManager.octaves = Mathf.RoundToInt(detailSlider.value);
        GameManager.eccentricity = ecenSlider.value;
    }
}
