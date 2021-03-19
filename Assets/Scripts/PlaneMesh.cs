using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/************************************
 * Plane mesh generation script
 * Written partially by me, as this script is a refactored and edited version of the one found in the cited Brackeys youtubre video.
 * This generates a mesh based on the values from the perlin noise array, then assigns color based on the normalised heights of the mesh.
 * **********************************/
public class PlaneMesh : MonoBehaviour
{
    Mesh mesh; //the generated mesh
    Vector3[] VertcieLocations; //locations of each vertecie
    int[] meshTriangles; //the triangles of each vertecie
    Color[] colors; //the array that holds the color map
    NoiseGeneration noiseGeneration; //the noiseGeneration script

    //values that store the desired size of the terrain
    private int xSize; 
    private int zSize;
    //noisemap made by the noiseGeneration script
    private float[,] noiseMap;

    public Gradient gradient; //gradient for the color
    //the highest and lowest point in the terrain
    float minHight; 
    float maxHeight;

    public void CreateTerrain(int width, int length)
    {
        //find the mesh and noiseGeneration script
        mesh = new Mesh();
        mesh = FindObjectOfType<MeshFilter>().mesh;
        noiseGeneration = FindObjectOfType<NoiseGeneration>();
        //find the size
        xSize = width;
        zSize = length;
        //define the size of each array
        VertcieLocations = new Vector3[(xSize + 1) * (zSize + 1)];
        noiseMap = noiseGeneration.Generate(xSize + 1, zSize + 1);
        //call functions secquentially
        GenerateVertArray(VertcieLocations);
        GenerateTriArray();
        AddColor();
        CreateMesh();
    }

    private void GenerateVertArray(Vector3[] vertLoactions)
    {
        //desired height from the perlin array
        float y = 0;
        //loop throguh the arrays
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //define y based on position in the array
                y = (noiseMap[z, x]) * 10;
                //add the vertecie
                vertLoactions[i] = new Vector3(x, y, z);
                //check to see if new value is the new highest or lowest point. If it is, redife highest and lowest
                if (y > maxHeight)
                    maxHeight = y;
                if (y < minHight)
                    minHight = y;
                //count up I, which represents the position in the verLocations array
                i++;
            }
        }

    }

    //create the triangles based on locations of the vertecies.
    private void GenerateTriArray()
    {
        //define the size of the array
        meshTriangles = new int[6 * (zSize * xSize)];
        //define and set the counter variables
        int arrayPosition = 0;
        int currentVert = 0;
        //loop throguh the array
        for (int p = 0; p < xSize; p++)
        {
            for (int i = 0; i < xSize; i++)
            {
                //call the addquad function to add 2 triangles to the array, creating a quad
                AddQuad(arrayPosition, currentVert);
                //update positions
                currentVert++;
                arrayPosition += 6;
            }
            currentVert++;
        }
    }

    private void AddQuad(int arrayPos, int currentVert)
    {
        //add one in the same patter every time, going around 3 vertecies and then going around the opposite 3 vertecies. 
        //That save those vertecies' locations which can then be used to create the mesh.
        meshTriangles[arrayPos] = currentVert + 0;
        meshTriangles[arrayPos + 1] = currentVert + xSize + 1;
        meshTriangles[arrayPos + 2] = currentVert + 1;

        meshTriangles[arrayPos + 3] = currentVert + 1;
        meshTriangles[arrayPos + 4] = currentVert + xSize + 1;
        meshTriangles[arrayPos + 5] = currentVert + xSize + 2;
    }

    private void AddColor()
    {
        //define an array of colors the same size as the vertecie count
        colors = new Color[VertcieLocations.Length];
        //loop through the array
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //normalise the value based on the highest and lowest points in the terrain
                float height = Mathf.InverseLerp(minHight, maxHeight, VertcieLocations[i].y);
                //set one of the colors in the array to the equivilant value in the gradient
                colors[i] = gradient.Evaluate(height);
                //count up i
                i++;
            }
        }
    }

    private void CreateMesh()
    {
        //clear whatever used to be there
        mesh.Clear();
        //calculate the vertecies based on the array defining their positions
        mesh.vertices = VertcieLocations;
        //calculate the triangles based on the array defining their positions
        mesh.triangles = meshTriangles;
        //set the color of each spot to the corresponding color defined in the colors array
        mesh.colors = colors;
        //recalculate the mesh
        mesh.RecalculateNormals();
        //find the mesh's collider and apply it to the mesh
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        //recalcuate the bounds of the collider to correspond with the new terrain
        mesh.RecalculateBounds();

    }


}