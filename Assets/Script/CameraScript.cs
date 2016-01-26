using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    Quaternion iniRot;
    
    // Use this for initialization
    void Start () {
        iniRot = transform.rotation;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation = iniRot;
    }
}
