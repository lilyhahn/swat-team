using UnityEngine;
using System.Collections;

public class Borders : MonoBehaviour {
    public float z = -5f;
    public float transitionSpeed = 1f;
    public Color color = Color.red;

    Vector3[] currentVerts;
    Vector3[] targetVerts;
    Material lineMaterial;

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

    void OnPostRender() {
        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(color);
        Vector3[] verts = new Vector3[targetVerts.Length];
        for(int i = 0; i < targetVerts.Length; i++){
            verts[i] = Vector3.Lerp(currentVerts[i], targetVerts[i], Time.deltaTime * transitionSpeed);
        }
        foreach (Vector3 vert in verts) {
            GL.Vertex(vert);
        }
        GL.End();
        GL.PopMatrix();
        if (Vector3.Distance(targetVerts[0], currentVerts[0]) <= 0.01) {
            currentVerts = targetVerts;
        }
    }

    public void DrawBorders(BoxCollider2D obj) {
        Bounds bounds = obj.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        targetVerts = new Vector3[]{
                              new Vector3(center.x - extents.x, center.y + extents.y, z),
                              new Vector3(center.x + extents.x, center.y + extents.y, z),
                              new Vector3(center.x + extents.x, center.y - extents.y, z),
                              new Vector3(center.x - extents.x, center.y - extents.y, z)
                          };
    }
}
