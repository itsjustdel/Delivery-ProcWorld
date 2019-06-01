using UnityEngine;
using System.Collections;

public class ShrinkCells : MonoBehaviour {
    public float shrinkAmtLerp = 0.1f;
	// Use this for initialization
	void Start ()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (i == 0)
                continue;

            verts[i] = Vector3.Lerp(verts[i], verts[0], shrinkAmtLerp); 
        }

        mesh.vertices = verts;

	}
	
	
}
