using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FillDropdown : MonoBehaviour
{
    //accesses the hex grid controls so that it can changed which level will be loaded when the load button is pressed
    private HexGrid grid;

    //stores the dropdown menu so it can be changed when new levels are created
    private Dropdown drop;

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("HexGrid").transform.GetComponent<HexGrid>();
        drop = transform.GetComponent<Dropdown>();

        GatherLevelNames(true);

        //adds a listener that checks whenever the user has changed the option in the dropdown menu
        drop.onValueChanged.AddListener(delegate { DropdownItemSelected(drop); });
    }

    //gets the new value it was changed too and changes the which level will be loaded when the user presses the load button
    private void DropdownItemSelected(Dropdown drop)
    {
        int index = drop.value;
        grid.ChangeLevelToLoad(drop.options[index].text);
    }

    //executes when a new level has been saved
    public void addLevel()
    {
        //deletes all of the old options due to bugs and errors with just simply adding them
        drop.options.Clear();

        GatherLevelNames(false);
    }
    private void GatherLevelNames(bool isGatheringAtStart)
    {
        //stores and gathers all of the JSON files with level data
        Object[] LevelFiles;
        LevelFiles = Resources.LoadAll("Level Files", typeof(TextAsset));

        //Gathers the names of each JSON file and adds them as an option to the dropdown menu
        foreach (TextAsset t in LevelFiles)
        {
            drop.options.Add(new Dropdown.OptionData() { text = t.name });
        }

        if(isGatheringAtStart)
        {
            //sets the default value to the first level
            grid.ChangeLevelToLoad(LevelFiles[0].name);
        }
    }
}
