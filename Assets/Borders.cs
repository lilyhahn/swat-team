using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Borders : MonoBehaviour {
    public float z = -5f;
    public float transitionSpeed = 1f;
    public Color color = Color.red;
    public float width = 1;
    public Texture2D lineTex;

    Vector2[] currentVerts;
    Vector2[] targetVerts;
    Material lineMaterial;

    void Awake() {
        currentVerts = new Vector2[]{
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0)
        };
        targetVerts = new Vector2[]{
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0)
        };
        lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
           "SubShader { Pass { " +
           "    Blend SrcAlpha OneMinusSrcAlpha " +
           "    ZWrite Off Cull Off Fog { Mode Off } " +
           "    BindChannels {" +
           "      Bind \"vertex\", vertex Bind \"color\", color }" +
           "} } }");
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnGUI() {
        for(int i = 0; i < currentVerts.Length; i++) {
            currentVerts[i] = Vector3.Lerp(currentVerts[i], targetVerts[i], Time.deltaTime * transitionSpeed);
        }
        DrawLine(Camera.main.WorldToScreenPoint(currentVerts[0]), Camera.main.WorldToScreenPoint(currentVerts[1]));
        DrawLine(Camera.main.WorldToScreenPoint(currentVerts[1]), Camera.main.WorldToScreenPoint(currentVerts[2]));
        DrawLine(Camera.main.WorldToScreenPoint(currentVerts[2]), Camera.main.WorldToScreenPoint(currentVerts[3]));
        DrawLine(Camera.main.WorldToScreenPoint(currentVerts[3]), Camera.main.WorldToScreenPoint(currentVerts[0]));
    }

    //void OnPostRender() {
    //    GL.PushMatrix();
    //    lineMaterial.SetPass(0);
    //    GL.LoadOrtho();
    //    GL.Begin(GL.LINES);
    //    GL.Color(color);
    //    for(int i = 0; i < currentVerts.Length; i++) {
    //        currentVerts[i] = Vector3.Lerp(currentVerts[i], targetVerts[i], Time.deltaTime * transitionSpeed);
    //    }
    //    List<Vector3> verts = new List<Vector3>();
    //    for(int i = 0; i < targetVerts.Length; i++){
    //        verts.Add(currentVerts[i]);
    //        //verts.Add(new Vector3(currentVerts[i].x, currentVerts[i].y + width, z));
    //    }
    //    foreach (Vector3 vert in verts) {
    //        GL.Vertex(vert);
    //    }
    //    GL.End();
    //    GL.PopMatrix();
    //    //if (Vector3.Distance(targetVerts[0], currentVerts[0]) <= 0.01) {
    //    //    currentVerts = targetVerts;
    //    //}
    //}

    public void DrawBorders(BoxCollider2D obj) {
        Bounds bounds = obj.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        targetVerts = new Vector2[]{
                              new Vector2(-extents.x, extents.y),
                              new Vector2(extents.x, extents.y),
                              new Vector2(extents.x, -extents.y),
                              new Vector2(-extents.x, -extents.y)
                          };
    }

    // from http://wiki.unity3d.com/wiki/index.php?title=DrawLine
    public void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;
 
        // Generate a single pixel texture if it doesn't exist
        if (!lineTex) { lineTex = new Texture2D(1, 1); }
 
        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;
 
        // Determine the angle of the line.
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
 
        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) { angle = -angle; }
 
        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
 
        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);
 
        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
 
        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}
