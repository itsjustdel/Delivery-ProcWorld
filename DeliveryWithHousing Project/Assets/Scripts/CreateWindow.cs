    using UnityEngine;
using System.Collections;


public class CreateWindow : ProcBase {


	void Start()
	{

		HouseVariables hV;
		hV = GetComponent<HouseVariables>();

		MeshBuilder meshBuilder = new MeshBuilder();
		BodyVariables house =hV.body;
		
		WindowVariables window = hV.window;

//		Vector3 upDir = (hV.body.lookRot* Vector3.up) * hV.body.Height;
//		Vector3 rightDir = (hV.body.lookRot* Vector3.right) * hV.body.Width;
//		Vector3 forwardDir = (hV.body.lookRot* Vector3.forward) * hV.body.Length;
		//Vector3 offset = (-rightDir + forwardDir) * 0.5f + ((forwardDir/hV.body.bricksX)*1.5f) ;

		house.lookRot = Quaternion.identity;
		window.position = new Vector3(house.Length*hV.window.windowOffset,(house.Height)*0.5f,(house.Width*0.5f)+ window.strutDepth); //X is length, Y is Height, Z is width
		
		window.windowSize = (hV.body.Height / hV.body.bricksY)* window.windowSize;
		window.windowSize*=0.8f;

			if (hV.body.windowAmount == 1 ){
			
				BuildWindow(meshBuilder,house.Height,house.Width,house.Length,window.windowSize,window.strutWidth,window.strutDepth,
			                  window.amountOfPanesX,window.amountOfPanesY,window.position,house.lookRot*Quaternion.Euler(0,270,0),hV);

				GameObject child = new GameObject();
				child.name = "Window";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
				
				Mesh mesh = meshBuilder.CreateMesh();
				meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;
				
				//Renderer renderer = GetComponent<MeshRenderer>().GetComponent<Renderer>();
				Material newMat = Resources.Load("Red",typeof(Material)) as Material;
				//renderer.material = newMat;
				meshRenderer.material = newMat;
				//return meshBuilder.CreateMesh();
			}

		if (hV.body.windowAmount == 2 ){
	//		BuildWindow(meshBuilder,house.Height,house.Width,house.Length,window.windowSize,window.strutWidth,window.strutDepth,
	//		            window.amountOfPanesX,window.amountOfPanesY,window.position,house.lookRot*Quaternion.Euler(0,90,0),hV);

			BuildWindow2(meshBuilder,house.Height,house.Width,house.Length,window.windowSize,window.strutWidth,window.strutDepth,
			            window.amountOfPanesX,window.amountOfPanesY,window.position,house.lookRot*Quaternion.Euler(0,270,0),hV);
			
			GameObject child = new GameObject();
			child.name = "Windows";
			child.transform.position = this.gameObject.transform.position;
			child.transform.parent = this.gameObject.transform;
			MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
			meshCollider.convex = true;
			meshCollider.isTrigger = true;

			//MeshCollider boxCollider = child.gameObject.AddComponent<MeshCollider>();

			MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
			
			Mesh mesh = meshBuilder.CreateMesh();
			meshCollider.sharedMesh = mesh;
			meshFilter.sharedMesh = mesh;
			
			//Renderer renderer = GetComponent<MeshRenderer>().GetComponent<Renderer>();
			Material newMat = Resources.Load("Red",typeof(Material)) as Material;
			//renderer.material = newMat;
			meshRenderer.material = newMat;
			//return meshBuilder.CreateMesh();
		

			child.gameObject.AddComponent<DestroyBrick>();
		}
				



	
	}

}
