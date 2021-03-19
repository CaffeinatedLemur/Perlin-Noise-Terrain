using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/************************************
 * Noise generation script
 * Written by me, generates a 2d array of floats using perlin noise. 
 * The noise becomes more or less detaialed by using a user-defined number of octaves.
 * **********************************/
public class NoiseGeneration : MonoBehaviour
{ 
    //x and y values for storing the perlin noise input values
    private float x;
    private float y;
    //temp values for storing the noisemap data when running through octave loop
    private float temp;
    public float[,] Generate(float xSize, float zSize)
    {
        //arrays of possible values for each octave
        float[] octaveHeights = new float[] { 1, 0.25f, 0.15f, 0.05f, 0.03f, 0.02f};
        float[] octaveCounts = new float[] {  1, 2f,    4f,    8f,    16f,   32f};
        float[] octaveFrequency = new float[] { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f };
        //the 2d array of perlin noise that will be returned
        float[,] noise = new float[(int)xSize, (int)zSize];

        //reset temp
        temp = 0;
        //loop throguh the array
        for (int i = 0; i < xSize; i++)
        {
            for (int p = 0; p < zSize; p++)
            {
                //calculate x and y based on how many mountains the user wants there to be
                x = (i / GameManager.scale);
                y = (p / GameManager.scale);
                //make more noise and overlay it for each octave defined by the user
                for (int octI = 0; octI < GameManager.octaves; octI++)
                    //calculate a single noise value and add it to the preveous for each octave, using a differenct set of values from the array each time. Also inclueds the seed from gameManager so that the user can restore previous perlin patterns
                    temp += (octaveHeights[octI] * (Mathf.PerlinNoise(octaveCounts[octI] * ((GameManager.seed + (x * octaveFrequency[octI]))), octaveCounts[octI] * (GameManager.seed + (y * octaveFrequency[octI])))));
                //raise temp the the power of how eccentric the user wants the mountains to be. The higher the defined number, the more extreme the mountians will be and the flatter the other terrain
                temp = Mathf.Pow(temp, GameManager.eccentricity);
                //set teh current location in the array to the temp perlin value
                noise[i, p] = temp;
                //reset temp
                temp = 0;
            }
        }
        //exit loop and return the finished array
        return noise;
    }

   
}