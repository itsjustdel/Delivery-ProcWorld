using UnityEngine;
using System.Collections;

public class ApplyCollider : MonoBehaviour {

    MeshFilter meshFilter;
    MeshCollider meshCollider;
	// Use this for initialization
	void Start () {
    meshFilter  = gameObject.GetComponent<MeshFilter>();
	meshCollider = gameObject.GetComponent<MeshCollider>();
    meshCollider.sharedMesh = meshFilter.mesh;

        this.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        meshCollider.sharedMesh = meshFilter.mesh;

    }
}
