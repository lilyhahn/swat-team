using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Borders : MonoBehaviour {
    public float z = -5f;
    public float transitionSpeed = 1f;
    public float width = 1f;

    List<LineRenderer> lines = new List<LineRenderer>();
    
    Vector3[] currentVerts;
    Vector3[] targetVerts;

    void Awake() {
        currentVerts = new Vector3[]{
            new Vector3(0, 0, z),
            new Vector3(0, 0, z),
            new Vector3(0, 0, z),
            new Vector3(0, 0, z)
        };
        targetVerts = new Vector3[]{
            new Vector3(0, 0, z),
            new Vector3(0, 0, z),
            new Vector3(0, 0, z),
            new Vector3(0, 0, z)
        };
        foreach(Transform child in transform){
            if(child.GetComponent<LineRenderer>() != null){
                lines.Add(child.GetComponent<LineRenderer>());
            }
        }
        foreach(LineRenderer line in lines){
            line.SetWidth(width, width);
        }
    }
    
    void Update(){
        for(int i = 0; i < currentVerts.Length; i++) {
            currentVerts[i] = Vector3.Lerp(currentVerts[i], targetVerts[i], Time.deltaTime * transitionSpeed);
        }
        //currentVerts[0] = new Vector3(currentVerts[0].x - width / 2, currentVerts[0].y, currentVerts[0].z);
        //currentVerts[2] = new Vector3(currentVerts[2].x + width / 2, currentVerts[2].y, currentVerts[2].z);
        for(int i = 0; i < lines.Count; i++){
            for(int j = 0; j < 2; j++){
                lines[i].SetPosition(j, currentVerts[(i + j) % lines.Count]);
            }
        }
    }

    public void DrawBorders(BoxCollider2D obj) {
        Bounds bounds = obj.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        Vector3 position = obj.transform.position;
        targetVerts = new Vector3[]{
                              new Vector3(-extents.x + position.x, extents.y + center.y, z),
                              new Vector3(extents.x + position.x, extents.y + center.y, z),
                              new Vector3(extents.x + position.x, -extents.y + center.y, z),
                              new Vector3(-extents.x + position.x, -extents.y + center.y, z)
                          };
    }
}
