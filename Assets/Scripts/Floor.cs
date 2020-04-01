using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public GameObject pitcher;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ball")){
            Debug.Log(other.gameObject.transform.position.z);
            GameObject currentPitcher = GameObject.FindWithTag("Pitcher");
            Destroy(currentPitcher);
            Destroy(other.gameObject);
            Instantiate(pitcher);
        }
    }
}
