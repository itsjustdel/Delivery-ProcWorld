using UnityEngine;
using System.Collections;

public class SmoothRoad : MonoBehaviour {

	public MeshFilter meshfilter;
	public Transform cube;
	// Use this for initialization

	void Awake (){

		enabled =false;

	}
	void Start () {
	

		Mesh mesh = meshfilter.mesh;

		for (int i = 0; i < mesh.vertexCount; i++)
		{
			Transform item = Instantiate(cube) as Transform;
			item.position = mesh.vertices[i];
			item.name = "mesh" + i;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
