using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/***********************************
 * TerrainEditor script
 * Written by me. Calls the functions that get the party started when the game starts or when the user presses the button.
 * *********************************/
public class TerrianEditor : MonoBehaviour
{
   //the scripts with the functions
    PlaneMesh planeMesh;
    InputHandler inputHandler;
    // Start is called before the first frame update
    //find the scripts and then generate the inital terrain using default values
    void Start()
    {

        planeMesh = FindObjectOfType<PlaneMesh>();
        inputHandler = FindObjectOfType<InputHandler>();
        EditTerrain();
    }

    public void EditTerrain()
    {
        //get the data the user input
        inputHandler.GetData();
        //create terrain at the maximum size
        planeMesh.CreateTerrain(256, 256);
    }
}
