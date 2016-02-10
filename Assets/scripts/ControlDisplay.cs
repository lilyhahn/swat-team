using UnityEngine;
using System.Collections;

public class ControlDisplay : MonoBehaviour {

	public GameObject joystickControls;
	public GameObject keyboardControls;
    public GameObject comboControls;
    SpriteRenderer currentControls;
    public bool fade = false;
    public float fadeDelay = 5f;
    public float fadeTime = 0.25f;
    
	// Use this for initialization
    void Start() {
        switch (Input.GetJoystickNames().Length) {
            case 0:
                keyboardControls.SetActive(true);
                joystickControls.SetActive(false);
                currentControls = keyboardControls.GetComponent<SpriteRenderer>();
                break;
            case 1:
                if (comboControls != null) {
                    comboControls.SetActive(true);
                    keyboardControls.SetActive(false);
                    joystickControls.SetActive(false);
                    currentControls = comboControls.GetComponent<SpriteRenderer>();
                    break;
                }
                else goto case 2; //fuck c#
            case 2:
                joystickControls.SetActive(true);
                keyboardControls.SetActive(false);
                currentControls = joystickControls.GetComponent<SpriteRenderer>();
                break;
            default:
                joystickControls.SetActive(true);
                keyboardControls.SetActive(false);
                comboControls.SetActive(false);
                currentControls = joystickControls.GetComponent<SpriteRenderer>();
                break;
        }
    }
    
    public void Fade(){
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine(){
        yield return new WaitForSeconds(fadeDelay);
        LeanTween.alpha(currentControls.gameObject, 0f, fadeTime).setOnComplete(delegate(){ Destroy(gameObject); } );
    }
	
}
