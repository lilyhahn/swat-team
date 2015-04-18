using UnityEngine;
using System.Collections;

public class ControlDisplay : MonoBehaviour {

	public GameObject joystickControls;
	public GameObject keyboardControls;

	// Use this for initialization
	void Start () {
		if(Input.GetJoystickNames().Length > 0){
			joystickControls.SetActive(true);
			keyboardControls.SetActive(false);
		}
		else{
			keyboardControls.SetActive(true);
			joystickControls.SetActive(false);
		}
	}
	
}
