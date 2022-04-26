using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RadialMenuButton : MonoBehaviour
{
    //stores the colors of the buttons when selected or not
    public Color SelectedColor;
    public Color NormalColor;

    //stores the image compenent that will be changed into either the selected or normal color
    private Image background;

    //accesses the hex paint data that will be changed in this script
    private HexPaintData hexStorage;
    
    //stores the hex rottation vallue tied to this button
    [SerializeField]
    private int rot = 0;

    //stores the hex prefab tied to this button
    [SerializeField]
    private Transform ButtonPrefab;

    void Start()
    {
        background = transform.GetComponent<Image>();
        hexStorage = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<HexPaintData>();

        //sets the color of the button to the regular color by default
        background.color = NormalColor;
       
    }

    //executes when the button is highlighted
    public void Selected()
    {
        background.color = SelectedColor;

        //checks if the button is a part of the Hex radial menu by checking if it has a button prefab attached to it
        if (ButtonPrefab != null)
        {
            //chnages the hex prefab stored in the hex painting data to the fprefab stored in this button
            hexStorage.ChangePrefab(ButtonPrefab);
        }

        //changes the rotation value stored in the hex painting data to the rotation value stored in this button
        else
        {
            hexStorage.ChangeRotation(rot);
        }
    }

    //excutes when the button is no longer highlighted and changes the color back to normal
    public void Deselected()
    {
        background.color = NormalColor;
        
    }
}
