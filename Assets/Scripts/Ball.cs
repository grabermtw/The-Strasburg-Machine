using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int ballID;
    private LearningManager manager;

    void Start()
    {
        manager = GameObject.FindWithTag("LearningManager").GetComponent<LearningManager>();
    }

/*
    void Update()
    {
        if(transform.position.y < 0)
        {
            manager.BallHitGround(ballID, transform.position);
        }
    }
    */
}
