﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private LearningManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<LearningManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Handle when the ball hits the ground
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            float distance = other.gameObject.transform.position.z;
            // Get rid of this ball
            Destroy(other.gameObject);
            // Inform the manager of the distance that the ball traveled in the z direction
            manager.BallHitGround(distance);
            
        }
    }
}
