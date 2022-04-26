using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexEdit : MonoBehaviour
{
    //stores the material so the color of the hex can be changed when highlighting it
    private Renderer materialSettings;

    //accesses the data stored for the hex painting
    private HexPaintData hexStorage;

    //stores data managing tthe hex grid including the creation and deletion of hexes 
    private HexGrid grid;

    //stops the hex from changing color or being changed when a menu is open
    private bool editingDisabled;

    //when enabled the hexes rotation is changed
    private bool rotatingHex;

    //stores the position of the hex in grid units. For example, a hex in the 5th row and 4th column would be x:4,y:5
    private Vector2 gridPostion;

    //stores the name of the prefab that uses this script
    public string prefabName;
    void Start()
    {
        materialSettings = transform.GetComponent<Renderer>();
        materialSettings.material.shader = Shader.Find("HDRP/Lit");
        hexStorage = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<HexPaintData>();
        grid = GameObject.FindGameObjectWithTag("HexGrid").transform.GetComponent<HexGrid>();
        editingDisabled = false;
        rotatingHex = false;
    }

    //used when instantiating the hex to store the position in hex units
    public void setGridPosition(Vector2 pos)
    {
        gridPostion = pos;
    }

    //allows access to the grid position
    public Vector2 getGridPosition()
    {
        return gridPostion;
    }

    //allows access to the rotation in degrees so it can be stored whhen saving a level
    public Vector3 getRotation()
    {
        return transform.localRotation.eulerAngles;
    }

    public void IsEditingDisabled(bool answer)
    {
        editingDisabled = answer;
    }

    private void Update()
    {
        //sends the transform of this object into the HexGrid and rotates it based on which rotation value is selected in the menu
        if (rotatingHex == true)
        {
            grid.RotateHex(transform);
        }
    }

    //executes when the mouse is hovering over the hex
    private void OnMouseOver()
    {
        //checks if a menu is not open
        if (!editingDisabled)
        {
            // checks if Left shift is held down
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //changes the color of the hex to have a green tint to show it is selected and will be changed 
                materialSettings.material.SetColor("_BaseColor", new Color(141 / 255f, 200 / 255f, 123 / 255f));

                //checks if the left mouse button is held down while left shift is held down
                if (Input.GetMouseButton(0))
                {
                    //creates a new hex based on which one was selected in the menu, it has the same positional values as this current hex
                    Transform hex = Instantiate(hexStorage.getPrefab()) as Transform;
                    hex.position = transform.position;
                    hex.parent = transform.parent;
                    hex.Rotate(new Vector3(0, 0, 1 * hexStorage.getRotation()));
                    hex.name = this.name;
                    hex.transform.GetComponent<HexEdit>().setGridPosition(gridPostion);
                    hex.transform.GetComponent<HexEdit>().prefabName = hexStorage.getPrefab().name;

                    //after creating the new hex, it destroys this one
                    Destroy(gameObject);
                }

                //checks if the middle mouse button is held down while left shift is held down
                if (Input.GetMouseButton(2))
                {
                    //disables all hexes except this one from being edited and allows the user to rotate the selected hex
                    grid.DisableHexes(transform.GetComponent<HexEdit>());
                    rotatingHex = true;
                }

                //enables alll the hexes and disables roation once the middle mouse button is released
                else
                {
                    rotatingHex = false;
                    grid.EnableHexes();
                }
            }

            else
            {
                //enables alll the hexes and disables roation once Left Shift is released
                rotatingHex = false;
                grid.EnableHexes();

                //chnages the color back to it's original color
                materialSettings.material.SetColor("_BaseColor", new Color(1f, 1f, 1f));
            }
        }
    }

    //chnages the color back to it's original color once the mouse is no longer hovering oveer the hex
    private void OnMouseExit()
    {
        materialSettings.material.SetColor("_BaseColor", new Color(1f, 1f, 1f));
    }

    




}
