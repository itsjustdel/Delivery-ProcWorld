using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TreeLine : MonoBehaviour {

	public Verge verge;
	public Transform vergeObject;
	public Transform cube;
	public int fieldDepth = 10;
	// Use this for initialization
	void Start () {

		StartCoroutine("Cubes");
	}
	
	IEnumerator Cubes()
	{
		MeshFilter meshfilter = vergeObject.GetComponent<MeshFilter>();
		Mesh mesh = meshfilter.sharedMesh;

		for (int i = 0; i < verge.xSize - Random.Range(0,50);i++)
		{
			Vector3 direction = mesh.vertices[ (mesh.vertices.Length-verge.xSize)+ i ] - 
								mesh.vertices[ (mesh.vertices.Length-(verge.xSize*2))+ i -1];

			Transform item = Instantiate(cube) as Transform;
			item.position = mesh.vertices[ (mesh.vertices.Length-verge.xSize)+ i ];
			Transform item2 = Instantiate(cube) as Transform;
			item2.position = mesh.vertices[ (mesh.vertices.Length-verge.xSize)+ i ] + (fieldDepth *direction);
			item2.name = "end of field";

		//	Transform item3 = Instantiate(cube) as Transform;
		//	item3.position = direction;
		//	item3.name = "direction";

			yield return new WaitForFixedUpdate();
		}
	}
}
