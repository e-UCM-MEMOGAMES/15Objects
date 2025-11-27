using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WriteGamemode : Gamemode
{
    [SerializeField]
    GameObject inputFieldObj;
    InputField inputField;


    // Start is called before the first frame update
    void Start()
    {
        inputField = inputFieldObj.GetComponent<InputField>();
    }

    private void OnEnable()
    {
        inputFieldObj.SetActive(false);
    }

    override public void Return()
    {
        inputFieldObj.SetActive(false);
    }

    override public void OnItemSelected(Collider2D[] items)
    {
        if (items.Length > 0)
        {
            inputFieldObj.SetActive(true);
        }
    }
}
