using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class RadialMenu : MonoBehaviour
{
    //stores the mouse position
    private Vector2 normalisedMousePosition;

    //stores the angle of the mouse position
    private float MouseAngle;

    //stores the value the array will use to make a selection
    private int selection;

    //stores the empty game object that holds all the hex buttons in the scene
    public GameObject hexButtonContainer;

    //stores the hex buttons in an array
    private RadialMenuButton[] hexButtons;

    //stores tthe amount of objects in the array
    private int hexButtonCount;

    ////stores the empty game object that holds all the rotation buttons in the scene
    public GameObject rotationButtonContainer;

    //stores the rotatiion buttons in an array
    private RadialMenuButton[] rotationButtons;

    //stores tthe amount of objects in the array
    private int rotationButtonCount;

    //accesses the hex paint data that will be changed in this script
    private HexPaintData hexStorage;

    void Start()
    {
        hexButtons = hexButtonContainer.transform.GetComponentsInChildren<RadialMenuButton>();
        rotationButtons = rotationButtonContainer.transform.GetComponentsInChildren<RadialMenuButton>();
        hexStorage = transform.GetComponent<HexPaintData>();
        StartCoroutine(ActiveDelay());
    }

    //adds a very slight delay so the start function has time to get all the components in the button arrays before the objeccts are disabled
    private IEnumerator ActiveDelay()
    {
        yield return new WaitForSeconds(0.1f);

        //disables the button containers so they arren't visibile and functional
        hexButtonContainer.SetActive(false);
        rotationButtonContainer.SetActive(false);

    }

    void Update()
    {
        //checks if the left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            //checks if left control is held down and then shows the buttons for the hex radial menu
            if (Input.GetKey(KeyCode.LeftControl))
            {
                hexButtonCount = hexButtons.Length;
                ShowButtons(hexButtonContainer, hexButtonCount, hexButtons);

                //shows the text in the middle of the menu displaying the name of the hex selected
                hexStorage.ShowSelectedText(true);
            }

            //hides the hex buttons once left control has beeen released
            else
            {
                hideButtons(hexButtonContainer, hexButtonCount, hexButtons);

                //hides the text in the middle of the menu displaying the name of the hex selected
                hexStorage.ShowSelectedText(false);   
            }     
        }

        //hides the hex buttons once the left mouse button has beeen released
        if (Input.GetMouseButtonUp(0))
        {
            hideButtons(hexButtonContainer, hexButtonCount, hexButtons);
        }

        //checks if the middle mouse button is held down and then shows the Rotation radial menu
        if (Input.GetMouseButton(2))
        {    
            rotationButtonCount = rotationButtons.Length;
            ShowButtons(rotationButtonContainer, rotationButtonCount, rotationButtons);
        }

        //hides the rotation buttons once the middle mouse butto has beeen released
        if (Input.GetMouseButtonUp(2))
        {
            hideButtons(rotationButtonContainer, rotationButtonCount, rotationButtons);
        }
    }

    //shows the buttons of the button container used for the function and is used to make a selection
    private void ShowButtons(GameObject container,int buttonCount, RadialMenuButton[] buttons)
    {
        //enables the container holding the buttons
        container.SetActive(true);

        //gets the mouse positon in relation to the centre of the screen. e.g. In a 1024 x 1024 screen 512,512 is acttually 0,0.
        normalisedMousePosition = new Vector2(Screen.width / 2 - Input.mousePosition.x, Input.mousePosition.y - Screen.height / 2);

        //gets the angle of the mouse in relation to the centre of the screen
        MouseAngle = Mathf.Atan2(normalisedMousePosition.y, normalisedMousePosition.x) * Mathf.Rad2Deg;

        //adjusts the mouse angle so that the top centre is 0 instead of centre left. It also makes sure the values are within 0-360 degrees
        MouseAngle = (MouseAngle + 270) % 360;

        //gets a number based on the angle and the amount of buttons. e.g. If there are 6 buttons, 0-60 would be 0, 60-120 would be 1, 120-180 would be 2 and so on.
        selection = (int)MouseAngle / (360 / buttonCount);

        //checks all of the buttons and checks which once in the array is selected and executes the function for that buttton
        for (int i = 0; i < buttonCount; i++)
        {
            if (selection == i)
            {
                buttons[i].Selected();
            }

            else
            {
                buttons[i].Deselected();
            }
        }
    }

    //disables all of the buttons in the container used in the function
    private void hideButtons(GameObject container, int buttonCount, RadialMenuButton[] buttons)
    {
        //sets all the buttons to deselected so they won't appear to be selected with a different color when the menu reopens
        for (int i = 0; i < buttonCount; i++)
        {
            buttons[i].Deselected();
        }

        container.SetActive(false);
    }


}
