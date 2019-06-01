using UnityEngine;
using System.Collections;


public class CreateBody : ProcBase {
	
	
	void Start(){
		
		HouseVariables hVScript;
		hVScript = GetComponent<HouseVariables>();
		
		MeshBuilder meshBuilder = new MeshBuilder();
		BodyVariables house =hVScript.body;
		TileVariables tiles = hVScript.tiles;
		WindowVariables window = hVScript.window;
		
		//Make main Body of House
		house.lookRot = Quaternion.identity; 
		//house.lookRot *= Quaternion.Euler(0,90,0);
		BuildBody(meshBuilder,house.Height,house.Width,house.Length,house.RoofHeight,house.RoofOverhangSide,
		           house.RoofOverhangFront,house.RoofBias,tiles.tilesX,tiles.tilesY,house.lookRot,hVScript);
		
		window.position = new Vector3(house.Length*hVScript.window.windowOffset,(house.Height)*0.5f,(house.Width*0.5f)+ window.strutDepth); //X is length, Y is Height, Z is width
		//BuildWindows(meshBuilder,window.windowSize,window.strutWidth,window.strutDepth,
		  //           window.amountOfPanesX,window.amountOfPanesY,window.position,house.lookRot*Quaternion.Euler(0,90,0));
		
		GameObject child = new GameObject();
		child.name = "Body";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();

		meshCollider.convex = true;

		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
		child.gameObject.AddComponent<Rigidbody>();
		child.GetComponent<Rigidbody>().isKinematic = true;
		FixedJoint joint = child.AddComponent<FixedJoint>();
		joint.connectedBody = this.gameObject.GetComponent<Rigidbody>();

		Mesh mesh = meshBuilder.CreateMesh();
		meshCollider.sharedMesh = mesh;
		meshFilter.sharedMesh = mesh;
	
		Material newMat = Resources.Load("Brown",typeof(Material)) as Material;

		meshRenderer.material = newMat;
		//child.transform.localScale*=0.9f;
//		return meshBuilder.CreateMesh();
	}		
		
}
	
