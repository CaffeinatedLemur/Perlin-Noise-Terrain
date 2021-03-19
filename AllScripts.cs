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
        float[] octaveHeights = new float[] { 1, 0.25f, 0.15f, 0.05f, 0.03f, 0.02f };
        float[] octaveCounts = new float[] { 1, 2f, 4f, 8f, 16f, 32f };
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

/***********************************************
 * Credits and Citations
 * All of the code was written by me. The PlaneMesh script was based off of the listed Brackeys youtube tutorial, thoguh it has been slgihtly modified and refactored since.
 * Sources for research:
 * vividosvividos 5. “Best Way to Add Seed to Perlin Noise?” Stack Overflow, 1 July 1960, stackoverflow.com/questions/7213469/best-way-to-add-seed-to-perlin-noise. 
 * Oliveira, Renan. “Complete Guide to Procedural Level Generation in Unity – Part 1.” GameDev Academy, 15 Mar. 2021, gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/#Creating_level_tile. 
 * Technologies, Unity. “Mathf.PerlinNoise Unity Docs.” Unity, docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html. 
 * Games, Red Blob. Making Maps with Noise Functions, www.redblobgames.com/maps/terrain-from-noise/. 
 * “How Do I Get Better Perlin Noise?” How Do I Get Better Perlin Noise? - Unity Answers, answers.unity.com/questions/1753537/how-do-i-get-better-perlin-noise.html. 
 * Technologies, Unity. “Mesh.” Unity, docs.unity3d.com/ScriptReference/Mesh.html. 
 * “Getting Started with Shader Graph.” Package Manager UI Website, docs.unity3d.com/Packages/com.unity.shadergraph@5.6/manual/Getting-Started.html. 
 * “Get Slider Value?” Get Slider Value? - Unity Answers, answers.unity.com/questions/875959/get-slider-value.html. 
 * Brackeys. “PROCEDURAL TERRAIN in Unity! - Mesh Generation.” YouTube, YouTube, 4 Nov. 2018, www.youtube.com/watch?v=64NblGkAabk. 
************************************************/