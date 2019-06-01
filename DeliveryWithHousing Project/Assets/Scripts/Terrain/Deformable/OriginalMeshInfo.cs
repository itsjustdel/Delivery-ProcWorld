using UnityEngine;
using System.Collections;

public class OriginalMeshInfo : MonoBehaviour {

    public Mesh originalMesh;

	// Use this for initialization
	void Start ()
    {
        originalMesh = new Mesh();
        originalMesh.vertices  = GetComponent<MeshFilter>().sharedMesh.vertices;
        originalMesh.triangles = GetComponent<MeshFilter>().sharedMesh.triangles;

        GetComponent<DeformController>().deformedVertices = originalMesh.vertices;
    }

}
