using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/***********************************
 * Game manager script
 * Written by me. Holds all the user-defined values in one location for all the scripts to access
 * *********************************/
public class GameManager : MonoBehaviour
{
    //seed that determines shape of perlin noise
    public static float seed = 34;
    //level of detail
    public static int octaves = 6;
    //how high the mountains are compared to rest of terrain
    public static float eccentricity = 5;
    //number of peaks
    public static float scale = 0.5f;
}
