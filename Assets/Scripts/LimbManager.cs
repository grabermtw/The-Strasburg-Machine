using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbManager : MonoBehaviour
{
    private Transform rightShoulder;
    private Transform rightArm;
    private Transform rightForeArm;
    private Transform rightHand;
    private float a;
    private float b;
    private float c;
    private float d;
    private float e;
    private float f;

    // Start is called before the first frame update
    void Start()
    {
        rightShoulder = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder");
        rightArm = rightShoulder.Find("mixamorig:RightArm");
        rightForeArm = rightArm.Find("mixamorig:RightForeArm");
        rightHand = rightForeArm.Find("mixamorig:RightHand");

        a = Random.Range(-5, 5);
        b = Random.Range(-5, 5);
        c = Random.Range(-5, 5);
        d = Random.Range(-5, 5);
        e = Random.Range(-5, 5);
        f = Random.Range(-5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        //rightShoulder.Rotate(new Vector3(0,1,0));
        rightArm.Rotate(new Vector3(a, b, c));
        rightForeArm.Rotate(new Vector3(d, e, f));
        //rightHand.Rotate(new Vector3(0,1,0));

    }
}
