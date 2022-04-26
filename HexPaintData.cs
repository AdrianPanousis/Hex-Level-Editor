using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexPaintData : MonoBehaviour
{
    //used to store the rotation value that will be used for a newly painted hex
    private int rotationOffset;

    //used to store the prefab that will be painted
    [SerializeField]
    private Transform PrefabSelected;

    //stores the text in the middle of the radial menu that will write the text of the prefab
    [SerializeField]
    private Text selectedText;

    [SerializeField]
    private Text selectedTextShadow;

  
    void Start()
    {
        rotationOffset = 0;
    }

    //used to display or remove the text
    public void ShowSelectedText(bool answer)
    {
        selectedText.enabled = answer;
        selectedTextShadow.enabled = answer;
    }

    //used to change the prefab that will be painted
    public void ChangePrefab(Transform prefab)
    {
        PrefabSelected = prefab;

        //changes the name of the selected text to the current prefab stored
        selectedText.text = prefab.name;
        selectedTextShadow.text = prefab.name;
    }

    //changes the rotation stored that will be used when painting a new hex
    public void ChangeRotation(int Rot)
    {
        rotationOffset = Rot;
    }

    //allows access to the prefab
    public Transform getPrefab()
    {
        return PrefabSelected;
    }

    //allows access to rotation
    public int getRotation()
    {
        return rotationOffset;
    }
}
