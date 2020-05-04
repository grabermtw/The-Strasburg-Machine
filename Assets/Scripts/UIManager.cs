using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public InputField numAjs;
    public InputField numThrows;
    public InputField parentsToKeep;
    public InputField crossoverProbability;
    public InputField mutationProbability;
    public Dropdown throwingStyle;

    private void Start() 
    {
        DontDestroyOnLoad(this);
        numAjs.text = PlayerPrefs.HasKey("numAjs") ? PlayerPrefs.GetInt("numAjs").ToString() : "10";
        numThrows.text = PlayerPrefs.HasKey("numThrows") ? PlayerPrefs.GetInt("numThrows").ToString() : "3";
        parentsToKeep.text = PlayerPrefs.HasKey("parentsToKeep") ? PlayerPrefs.GetFloat("parentsToKeep").ToString() : "0.2";
        crossoverProbability.text = PlayerPrefs.HasKey("crossoverProbability") ? PlayerPrefs.GetFloat("crossoverProbability").ToString() : "0.95";
        mutationProbability.text = PlayerPrefs.HasKey("mutationProbability") ? PlayerPrefs.GetFloat("mutationProbability").ToString() : "0.05";
        throwingStyle.value = PlayerPrefs.HasKey("throwingStyle") ? PlayerPrefs.GetInt("throwingStyle") : 0;
    }

    public void OnStartClicked()
    {
        PlayerPrefs.SetInt("numAjs", int.Parse(numAjs.text));
        PlayerPrefs.SetInt("numThrows", int.Parse(numThrows.text));
        PlayerPrefs.SetFloat("parentsToKeep", Mathf.Clamp(float.Parse(parentsToKeep.text), 0.0f, 1.0f));
        PlayerPrefs.SetFloat("crossoverProbability", Mathf.Clamp(float.Parse(crossoverProbability.text), 0.0f, 1.0f));
        PlayerPrefs.SetFloat("mutationProbability", Mathf.Clamp(float.Parse(mutationProbability.text), 0.0f, 1.0f));
        PlayerPrefs.SetInt("throwingStyle", throwingStyle.value);

        SceneManager.LoadScene(1);
    }


    
}
