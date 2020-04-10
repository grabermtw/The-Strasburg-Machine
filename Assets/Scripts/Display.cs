using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{

    public Text generationText;
    public static Display instance;

    // Start is called before the first frame update
    void Start()
    {
        // creates static instance the class will use and sets it equal to the first instance created
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        UpdateText(0,0);
    }

    public static void UpdateText(int generationNum, int throwNum)
    {
        instance.generationText.text = "Generation "+generationNum+"\nThrow "+(throwNum + 1);
    }
}
