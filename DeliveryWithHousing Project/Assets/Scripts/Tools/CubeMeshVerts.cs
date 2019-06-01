using UnityEngine;
using System.Collections;

public class CubeMeshVerts : MonoBehaviour {
	public Transform cubePrefab;

	void Awake()
	{
		enabled = false;
	}

	// Use this for initialization
	void Start () {
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

		for (int i = 0; i < mesh.vertexCount; i++)
		{
			Transform cube = Instantiate(cubePrefab,mesh.vertices[i],Quaternion.identity) as Transform;
			cube.name = i.ToString();
		}

	}
	

}
