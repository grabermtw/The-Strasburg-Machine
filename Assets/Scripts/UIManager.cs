using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject armOnlyManager;
    public GameObject fullBodyManager;
    public GameObject openingUI;
    public GameObject learningUI;

    public void BeginLearning(bool armOnly)
    {
        openingUI.SetActive(false);
        learningUI.SetActive(true);
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
