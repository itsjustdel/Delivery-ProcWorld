using UnityEngine;
using System.Collections;


public class CreateTestWall : ProcBase {

	void Start(){
		HouseVariables hV;
		hV = GetComponent<HouseVariables>();
		
		MeshBuilder meshBuilder = new MeshBuilder();
		BodyVariables house =hV.body;
		
		WindowVariables window = hV.window;

		BuildTestWall(meshBuilder,
		              house.Height,house.Width,
		              house.Length,
		              window.windowSize,
		              window.strutWidth,
		              window.strutDepth,
		              window.amountOfPanesX,
		              window.amountOfPanesY,
		              window.position,
		              house.lookRot*Quaternion.Euler(0,0,0),
		              hV);

		BuildTestWall2(meshBuilder,
		              house.Height,house.Width,
		              house.Length,
		              window.windowSize,
		              window.strutWidth,
		              window.strutDepth,
		              window.amountOfPanesX,
		              window.amountOfPanesY,
		              window.position,
		              house.lookRot*Quaternion.Euler(0,0,0),
		              hV);



		GameObject child = new GameObject();
		child.name = "TestWall";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
		
		Mesh mesh = meshBuilder.CreateMesh();
		meshCollider.sharedMesh = mesh;
		meshFilter.sharedMesh = mesh;
		
		//Renderer renderer = GetComponent<MeshRenderer>().GetComponent<Renderer>();
		Material newMat = Resources.Load("Brown",typeof(Material)) as Material;
		//renderer.material = newMat;
		meshRenderer.material = newMat;
		//return meshBuilder.CreateMesh();
	
	}
	            
}