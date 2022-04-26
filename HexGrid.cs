using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class HexGrid : MonoBehaviour
{
    //this class stores the data of each individual hex to be used when the JSON file is created when saving
    [System.Serializable]
    public class SingleHex
    {
        public Vector2 GridPosition;
        public Vector3 HexRotation;
        public string HexPrefab;
    }

    //this class stores the data of the entire hex grid to be used when the JSON file is created when saving
    [System.Serializable]
    public class HexLevel
    {
        public List<SingleHex> SingleHexes = new List<SingleHex>();
        public int gridWidth;
        public int gridHeight;
        public float gridGap;
    }

    //initialises grid to save
    private HexLevel currentGrid = new HexLevel();

    //stores the default prefab to be used when the initial grid and new rows and columns arre created
    public Transform hexPrefab;

    //stores the current grid dimensions
    public int gridWidth = 11;
    public int gridHeight = 11;

    //stores the text objects to display the curent grid height and width
    [SerializeField]
    private Text gridWidthText;

    [SerializeField]
    private Text gridHeightText;

    //the real world height and width of the hex
    float hexWidth = 1.732f;
    float hexHeight = 2.0f;

    private float hexWidthAndGap;
    private float hexHeightAndGap;

    //stores the value determining how much distance there is between hexes
    public float gap = 0.0f;

    //accesses the data stored for the hex painting
    private HexPaintData hexStorage;

    //stores the name of the level
    private string levelToLoad;

    void Start()
    {
        //creates the initial default grid and adds the default gap
        AddGapValue();
        CreateGrid();

        hexStorage = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<HexPaintData>();
        gridWidthText.text = gridWidth.ToString();
        gridHeightText.text = gridHeight.ToString();
    }

    //sets the gap value to be used when hexes are created or moved
    private void AddGapValue()
    {
        hexWidthAndGap = hexWidth + (hexWidth * gap);
        hexHeightAndGap = hexHeight + (hexHeight * gap);
    }

    //used to change the string that stores the file name of the level that will be loaded
    public void ChangeLevelToLoad(string n)
    {
        levelToLoad = n;
    }

    //changes the physical gap between hexes in real time
    public void ChangeGridGap(float g)
    {
        //changes the gap value
        gap = g;
        AddGapValue();
        
        //gets all of the hexes in the scene. Is called every time because the grid will constantly change 
        HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();

        //changes the position of each hex with the gap
        foreach (HexEdit hex in Hexes)
        {
            hex.transform.position = CalcWorldPos(hex.getGridPosition());
        }
    }

    //saves the level to a JSON file with all the relevant data stored
    public void SaveLevel(Text name)
    {
        //gets all of the hexes in the scene. Is called every time because the grid will constantly change 
        HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();

        //adds every single hex on the grid to the Hexlevel class and gathers data from each hex to be stored in the JSON file
        for(int i=0; i <Hexes.Length ;i++)
        {
            currentGrid.SingleHexes.Add(new SingleHex());
            currentGrid.SingleHexes[i].GridPosition = Hexes[i].getGridPosition();
            currentGrid.SingleHexes[i].HexRotation = Hexes[i].getRotation();
            currentGrid.SingleHexes[i].HexPrefab = Hexes[i].prefabName;
        }

        //gathers other data about the hex grid that isn't needed for an individual hex
        currentGrid.gridHeight = gridHeight;
        currentGrid.gridWidth = gridWidth;
        currentGrid.gridGap = gap;

        //creates a string that stores all of the JSON data
        string JsonOutput = JsonUtility.ToJson(currentGrid,true);
        
        //converts that string into a text file and stores it in the Level Files folder in Resources
        File.WriteAllText(Application.dataPath + "/Resources/Level Files/" + name.text + ".txt", JsonOutput);
    }

    //loads data from a selected JSON file
    public void LoadLevel()
    {
        //loads the text file that hold the data for the selected level
        TextAsset level;
        level = Resources.Load("Level Files/" + levelToLoad) as TextAsset;
        
        //destroys every hex in the grid
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        //gets the JSON data from the text file
        HexLevel newLevel = new HexLevel();
        newLevel = JsonUtility.FromJson<HexLevel>(level.text);

        //changes the gap before the hexes ae instantiated
        gap = newLevel.gridGap;
        AddGapValue();

        //creates all of the hexes in the JSOn file
        foreach (SingleHex h in newLevel.SingleHexes)
        {
            //checks if the data is not a blank entry due to errors in saving the JSON file
            if (h.HexPrefab != "")
            {
                //creates the hex
                GameObject hex = Instantiate(Resources.Load(h.HexPrefab)) as GameObject;
                Vector2 gridPos = new Vector2(h.GridPosition.x, h.GridPosition.y);

                //changes the position, rotation and name of the hex and makes sure it is a child of this object
                hex.transform.position = CalcWorldPos(gridPos);
                hex.transform.parent = this.transform;
                hex.name = "Hexagon" + h.GridPosition.x + "|" + h.GridPosition.y;
                hex.transform.localEulerAngles = h.HexRotation;

                //changes the values stored in the hex
                hex.transform.GetComponent<HexEdit>().setGridPosition(new Vector2(h.GridPosition.x, h.GridPosition.y));
                hex.transform.GetComponent<HexEdit>().prefabName = h.HexPrefab;
            }
            
        }

        //changes the grid width and height values stored in the JSON files and changes the text displayed in the GUI
        gridWidth = newLevel.gridWidth;
        gridWidthText.text = gridWidth.ToString();
        gridHeight = newLevel.gridHeight;
        gridHeightText.text = gridHeight.ToString();

    }

    //changes the grid width in incremennts of 1 or -1 every time it is called
    public void ChangeGridWidth(int n)
    {
        //changes the grid width value
        int w = gridWidth + n;

        //checks if the grid width has increased
        if (w > gridWidth)
        {
            //checks for every single row of the hex grid
            for (int y = 0; y < gridHeight; y++)
            {
                CreateHex(gridWidth, y);
            }
        }

        //checks if the grid width has decreased
        else
        {
            //gets all of the hexes in the scene. Is called every time because the grid will constantly change 
            HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();

            //destroys every hex in the column
            foreach (HexEdit hex in Hexes)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = w; x < gridWidth; x++)
                    {
                        if(hex.getGridPosition() == new Vector2(x,y))
                        {
                            Destroy(hex.gameObject);
                        }
                    }
                }
            }
                
        }

        //changes the grid width value and changes the value of the text displayed in the GUI
        gridWidth = w;
        gridWidthText.text = gridWidth.ToString();
    }

    //changes the grid height in incremennts of 1 or -1 every time it is called
    public void ChangeGridHeight(int n)
    {
        //changes the grid height value
        int h = gridHeight + n;

        //checks if the grid height has increased
        if (h > gridHeight)
        {
            //checks for every single row of the hex grid
            for (int x = 0; x < gridWidth; x++)
            {
                CreateHex(x, gridHeight);
            }
        }

        //checks if the grid height has decreased
        else
        {
            //gets all of the hexes in the scene. Is called every time because the grid will constantly change
            HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();

            //destroys every hex in the row
            foreach (HexEdit hex in Hexes)
            {
                for (int y = h; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (hex.getGridPosition() == new Vector2(x, y))
                        {
                            Destroy(hex.gameObject);
                        }
                    }
                }
            }
        }

        //changes the grid height value and changes the value of the text displayed in the GUI
        gridHeight = h;
        gridHeightText.text = gridHeight.ToString();
    }

    //disables all hexes in the scene from being edited and changed except the enabled hex entered into the arguments
    public void DisableHexes(HexEdit enabledHex)
    {
        //gets all of the hexes in the scene. Is called every time because the grid will constantly change
        HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();

        //disables all of the hexes
        foreach(HexEdit hex in Hexes)
        {
            if(hex != enabledHex)
            {
                hex.IsEditingDisabled(true);
            }
        }
    }

    //enables all hexes in the scene to be changed
    public void EnableHexes()
    {
        //gets all of the hexes in the scene. Is called every time because the grid will constantly change
        HexEdit[] Hexes = transform.GetComponentsInChildren<HexEdit>();
        //enables all of the hexes
        foreach (HexEdit hex in Hexes)
        {
            hex.IsEditingDisabled(false);
        }
    }

    //changes the rotation of the hex entered into the arguments to the hex rotation stored in hex storage
    public void RotateHex(Transform Hex)
    {
        Hex.transform.eulerAngles = new Vector3(-90, 0, hexStorage.getRotation());
    }

    //calculates the real world position for each hex to place them into the scene
    private Vector3 CalcWorldPos(Vector2 gridPos)
    {
        float offset = 0;

        //checks if the y position is apart of every second row and changes the x offset so it moves slightly to the right
        if (gridPos.y % 2 != 0)
        {
            offset = hexWidthAndGap / 2;
        }

        //gets the x and y values of the new world position
        float x = 0 + gridPos.x * hexWidthAndGap + offset;
        float z = 0 - gridPos.y * hexHeightAndGap * 0.75f;

        return new Vector3(x, 0, z);
    }

    //creates the initial grid basedon the default height and width
    private void CreateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CreateHex(x, y);
            }
        }

        
    }

    //instantiates the hex and positions it into the scene based on the x and y values
    private void CreateHex(int x,int y)
    {
        //creates the hex
        Transform hex = Instantiate(hexPrefab) as Transform;
        Vector2 gridPos = new Vector2(x, y);
        hex.position = CalcWorldPos(gridPos);
        hex.parent = this.transform;
        hex.name = "Hexagon" + x + "|" + y;
        hex.transform.GetComponent<HexEdit>().setGridPosition(new Vector2(x, y));
        hex.transform.GetComponent<HexEdit>().prefabName = hexPrefab.name;
    }
}
