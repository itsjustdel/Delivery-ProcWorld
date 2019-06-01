using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RemoveDegenerateTriangles : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	    Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] verts = mesh.vertices;


        for (int i = 0; i < verts.Length; i++)
        {

        }
	}
	
}
