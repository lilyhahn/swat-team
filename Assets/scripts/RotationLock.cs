using UnityEngine;
using System.Collections;

public class RotationLock : MonoBehaviour {

	public Vector3 rotation;
    
	// Update is called once per frame
	void Update () {
	   transform.eulerAngles = rotation;
	}
}
