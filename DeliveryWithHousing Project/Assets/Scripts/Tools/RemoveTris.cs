using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RemoveTris : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        List<int> tris = new List<int>();
        mesh.triangles = tris.ToArray();
	}
	
	
}
