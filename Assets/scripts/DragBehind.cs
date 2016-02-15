using UnityEngine;
using System.Collections;

public class DragBehind : MonoBehaviour {

    public float speed = 1;
    
    void Update(){
        transform.eulerAngles = Vector3.Lerp(transform.localPosition, transform.parent.localPosition, Time.deltaTime * speed);
    }
}
