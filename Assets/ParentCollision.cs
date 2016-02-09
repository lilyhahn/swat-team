using UnityEngine;
using System.Collections;

public class ParentCollision : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D c){
        transform.parent.GetComponent<Hand>().OnTriggerEnter2D(c);
    }
    
    void OnTriggerStay2D(Collider2D c){
        transform.parent.GetComponent<Hand>().OnTriggerStay2D(c);
    }
    
    void OnTriggerExit2D(Collider2D c){
        transform.parent.GetComponent<Hand>().OnTriggerExit2D(c);
    }
}
