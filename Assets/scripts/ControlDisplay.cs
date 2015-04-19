using UnityEngine;
using System.Collections;

public class ControlDisplay : MonoBehaviour {

	public GameObject joystickControls;
	public GameObject keyboardControls;
    public GameObject comboControls;

	// Use this for initialization
    void Start() {
        switch (Input.GetJoystickNames().Length) {
            case 0:
                keyboardControls.SetActive(true);
                joystickControls.SetActive(false);
                break;
            case 1:
                if (comboControls != null) {
                    comboControls.SetActive(true);
                    keyboardControls.SetActive(false);
                    joystickControls.SetActive(false);
                    break;
                }
                else goto case 2; //fuck c#
            case 2:
                joystickControls.SetActive(true);
                keyboardControls.SetActive(false);
                break;
            default:
                joystickControls.SetActive(true);
                keyboardControls.SetActive(false);
                comboControls.SetActive(false);
                break;
        }
    }
	
}
