using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOfLearningManager : MonoBehaviour
{

    public GameObject armOnlyManager;
    public GameObject fullBodyManager;

    // Start is called before the first frame update
    void Start()
    {
        BeginLearning(PlayerPrefs.GetInt("throwingStyle") == 0);
    }

    public void BeginLearning(bool armOnly)
    {
        //openingUI.SetActive(false);
        //learningUI.SetActive(true);
        if(armOnly){
            Instantiate(armOnlyManager);
        }
        else{
            Instantiate(fullBodyManager);
        }
        // Give the Floor its learning manager
        GameObject.FindWithTag("Floor").GetComponent<Floor>().AssignManager();
    }
}
