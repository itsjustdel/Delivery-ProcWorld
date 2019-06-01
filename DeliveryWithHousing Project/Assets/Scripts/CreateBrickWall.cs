using UnityEngine;
using System.Collections;

public class CreateBrickWall : ProcBase {

	// Use this for initialization
	void Start () 
	{
		HouseVariables hV;
		hV = GetComponent<HouseVariables>();
		//MeshBuilder meshBuilder = new MeshBuilder(); 

		Vector3 upDir = (hV.body.lookRot* Vector3.up) * hV.body.Height;
		Vector3 rightDir = (hV.body.lookRot* Vector3.right) * hV.body.Width;
		Vector3 forwardDir = (hV.body.lookRot* Vector3.forward) * hV.body.Length;
		Vector3 offset = (-rightDir + forwardDir) * 0.5f + ((forwardDir/hV.body.bricksX)*1.5f) ;

/////////////Build Long Parts of da Wall
		
		for (int i = 0; i <= hV.body.bricksX; i++)
		{
			
			offset -= (forwardDir)/hV.body.bricksX;
			
			for (int j = 0; j < (hV.body.bricksY/2); j++)
			{
				Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;

				MeshBuilder meshBuilder = new MeshBuilder(); 


				BuildBrickWall(meshBuilder,hV,offset,yOffset);

				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;


				Vector3 center = offset + yOffset - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;

				BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
				meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
				meshCollider.size -=hV.body.brickSeperation*Vector3.up;
		
				meshCollider.center = center;
				//meshCollider.convex = true;

				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
				Mesh mesh = meshBuilder.CreateMesh();

				//meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;	

				//Material randomMat = new Material(Shader.Find("Standard"));
				//Color newColor = new Color( Random.Range(-0.0f,0.0f),Random.Range(-0.10f,0.10f), Random.Range(-0.0f,0.0f),1.0f);
				//meshRenderer.material = randomMat;
				//Material newMat = Resources.Load("Brick",typeof(Material)) as Material;
				//Material comboColour = new Material(Shader.Find("Standard"));
				
				//float temp = newMat.color.g+newColor.g;
				//Color tempColor = new Color (newMat.color.r,temp,newMat.color.b,1.0f);
				//comboColour.color = tempColor;
				//meshRenderer.material =comboColour;
				meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;

				meshRenderer.shadowCastingMode = 0;
				meshRenderer.receiveShadows = false;
				//child.layer = 8;


				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.right,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.forward,Random.Range(-5.0f,5.0f));		

				child.gameObject.AddComponent<Rigidbody>();
				child.GetComponent<Rigidbody>().mass= 2;
				FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = gameObject.GetComponent<Rigidbody>();
				joint.enableCollision = true;
				child.gameObject.GetComponent<FixedJoint>().breakForce = hV.body.brickBreakForce;
				child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
				child.gameObject.tag= "SingleMesh";
				//child.GetComponent<Rigidbody>().detectCollisions = false;
				//child.gameObject.AddComponent<DestroyBrick>();
			}
		}

		for (int i = 0; i < hV.body.bricksX; i++)
		{
			
			offset += ((forwardDir)/hV.body.bricksX);
			//offset += ((forwardDir)/hV.body.bricksX)/2;
			for (int j = 0; j < (hV.body.bricksY/2)-1; j++)
			{
				Vector3 yOffset =  (upDir/hV.body.bricksY)+ ((upDir/hV.body.bricksY)*j*2);
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				

				BuildBrickWall(meshBuilder,hV,offset - (((forwardDir)/hV.body.bricksX)/2),yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				
				Vector3 center = offset + yOffset - ((forwardDir/hV.body.bricksX)) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
				meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
				meshCollider.size -=hV.body.brickSeperation*Vector3.up;
				
				meshCollider.center = center;
				//meshCollider.convex = true;
						
				
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
				Mesh mesh = meshBuilder.CreateMesh();
				
				//meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;	
				
				//Material randomMat = new Material(Shader.Find("Standard"));
				//Color newColor = new Color( Random.Range(-0.0f,0.0f),Random.Range(-0.10f,0.10f), Random.Range(-0.0f,0.0f),1.0f);
				//meshRenderer.material = randomMat;
				//Material newMat = Resources.Load("Brick",typeof(Material)) as Material;
				//Material comboColour = new Material(Shader.Find("Standard"));

				//float temp = newMat.color.g+newColor.g;
				//Color tempColor = new Color (newMat.color.r,temp,newMat.color.b,1.0f);
				//comboColour.color = tempColor;
				//meshRenderer.material =comboColour;

				meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;

				meshRenderer.shadowCastingMode = 0;
				meshRenderer.receiveShadows = false;
				//child.layer = 8;

				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.right,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.forward,Random.Range(-5.0f,5.0f));		
				
				child.gameObject.AddComponent<Rigidbody>();
				child.GetComponent<Rigidbody>().mass= 2;
				FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = gameObject.GetComponent<Rigidbody>();
				joint.enableCollision = true;
				child.gameObject.GetComponent<FixedJoint>().breakForce = hV.body.brickBreakForce;
				child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
				child.gameObject.tag= "SingleMesh";
				//child.GetComponent<Rigidbody>().detectCollisions = false;				
				//child.gameObject.AddComponent<DestroyBrick>();


			}
		}

		for (int i = 0; i <= hV.body.bricksX; i++)
		{
			
			offset -= (forwardDir)/hV.body.bricksX;
			
			for (int j = 0; j < (hV.body.bricksY/2); j++)
			{
				Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				

				//BuildBrickWall(meshBuilder,hV,offset+ rightDir,yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				
				Vector3 center = offset + yOffset - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;
				center += rightDir;

				BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
				meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
				meshCollider.size -=hV.body.brickSeperation*Vector3.up;
				
				meshCollider.center = center;
				//meshCollider.convex = true;
				

				
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
				Mesh mesh = meshBuilder.CreateMesh();
				
				//meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;	
				
				//Material randomMat = new Material(Shader.Find("Standard"));
				//Color newColor = new Color( Random.Range(-0.0f,0.0f),Random.Range(-0.10f,0.10f), Random.Range(-0.0f,0.0f),1.0f);
				//meshRenderer.material = randomMat;
				//Material newMat = Resources.Load("Brick",typeof(Material)) as Material;
				//Material comboColour = new Material(Shader.Find("Standard"));
				
				//float temp = newMat.color.g+newColor.g;
				//Color tempColor = new Color (newMat.color.r,temp,newMat.color.b,1.0f);
				//comboColour.color = tempColor;
				//meshRenderer.material =comboColour;

				meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;

				meshRenderer.shadowCastingMode = 0;
				meshRenderer.receiveShadows = false;
				//child.layer = 8;

				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.right,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.forward,Random.Range(-5.0f,5.0f));		

				child.gameObject.AddComponent<Rigidbody>();
				child.GetComponent<Rigidbody>().mass= 2;
				FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = gameObject.GetComponent<Rigidbody>();
				joint.enableCollision = true;
				child.gameObject.GetComponent<FixedJoint>().breakForce = hV.body.brickBreakForce;
				child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
				child.gameObject.tag= "SingleMesh";
				//child.GetComponent<Rigidbody>().detectCollisions = false;
				//child.gameObject.AddComponent<DestroyBrick>();
			}
		}

		for (int i = 0; i < hV.body.bricksX; i++)
		{
			
			offset += ((forwardDir)/hV.body.bricksX);

			//offset += 
				
			//offset += ((forwardDir)/hV.body.bricksX)/2;
			for (int j = 0; j < (hV.body.bricksY/2)-1; j++)
			{
				Vector3 yOffset =  (upDir/hV.body.bricksY)+ ((upDir/hV.body.bricksY)*j*2);
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				
				Vector3 brickZDir = (hV.body.Length/hV.body.bricksX) * Vector3.right;
				brickZDir *= hV.body.bricksZ;
				brickZDir += hV.body.Length/hV.body.bricksX * Vector3.right;

				BuildBrickWall(meshBuilder,hV,offset - (((forwardDir)/hV.body.bricksX)/2) + brickZDir ,yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				
				Vector3 center = offset + yOffset - ((forwardDir/hV.body.bricksX)) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;
				center+=rightDir;
				BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
				meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
				meshCollider.size -=hV.body.brickSeperation*Vector3.up;
				
				meshCollider.center = center;
				//meshCollider.convex = true;
				
						
				
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
				Mesh mesh = meshBuilder.CreateMesh();

				//meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;	
				
				//Material randomMat = new Material(Shader.Find("Standard"));
				//Color newColor = new Color( Random.Range(-0.0f,0.0f),Random.Range(-0.10f,0.10f), Random.Range(-0.0f,0.0f),1.0f);
				//meshRenderer.material = randomMat;
				//Material newMat = Resources.Load("Brick",typeof(Material)) as Material;
				//Material comboColour = new Material(Shader.Find("Standard"));
				
				//float temp = newMat.color.g+newColor.g;
				//Color tempColor = new Color (newMat.color.r,temp,newMat.color.b,1.0f);
				//comboColour.color = tempColor;
				//meshRenderer.material =comboColour;

				meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;

				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.right,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.forward,Random.Range(-5.0f,5.0f));	

				meshRenderer.shadowCastingMode = 0;
				meshRenderer.receiveShadows = false;
			//	child.layer = 8;
				
				child.gameObject.AddComponent<Rigidbody>();
				child.GetComponent<Rigidbody>().mass= 2;
				FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = gameObject.GetComponent<Rigidbody>();
				joint.enableCollision = true;
				child.gameObject.GetComponent<FixedJoint>().breakForce = hV.body.brickBreakForce;
				child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
				child.gameObject.tag= "SingleMesh";
				//child.GetComponent<Rigidbody>().detectCollisions = false;
				//child.gameObject.AddComponent<DestroyBrick>();
			}
		}

///////Build ends of da Wall


		float brickLength = hV.body.Length/hV.body.bricksX;
		float houseWidth = hV.body.bricksZ *hV.body.Length/hV.body.bricksX;

		Debug.Log(houseWidth);

		for (int i = 0; i < hV.body.bricksZ; i++)
		{			
			//offset -= (-rightDir)/hV.body.bricksX;
			offset -= (-brickLength*Vector3.right) ;
			
			for (int j = 0; j < (hV.body.bricksY/2); j++)
			{
				Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				
				
				BuildBrickWallSide(meshBuilder,hV,offset,yOffset);
				
				GameObject child = new GameObject();
				child.name = "BrickSide";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				
				Vector3 center = offset + yOffset - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
				meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
				meshCollider.size -=hV.body.brickSeperation*Vector3.up;
				
				meshCollider.center = center;
				//meshCollider.convex = true;
				
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
				Mesh mesh = meshBuilder.CreateMesh();
				
				//meshCollider.sharedMesh = mesh;
				meshFilter.sharedMesh = mesh;	
				
				//Material randomMat = new Material(Shader.Find("Standard"));
				//Color newColor = new Color( Random.Range(-0.0f,0.0f),Random.Range(-0.10f,0.10f), Random.Range(-0.0f,0.0f),1.0f);
				//meshRenderer.material = randomMat;
				//Material newMat = Resources.Load("Brick",typeof(Material)) as Material;
				//Material comboColour = new Material(Shader.Find("Standard"));
				
				//float temp = newMat.color.g+newColor.g;
				//Color tempColor = new Color (newMat.color.r,temp,newMat.color.b,1.0f);
				//comboColour.color = tempColor;
				//meshRenderer.material =comboColour;
				meshRenderer.sharedMaterial = Resources.Load("Brick",typeof(Material)) as Material;
				
				meshRenderer.shadowCastingMode = 0;
				meshRenderer.receiveShadows = false;
				//child.layer = 8;
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.right,Random.Range(-5.0f,5.0f));
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.forward,Random.Range(-5.0f,5.0f));		
				//child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,90);

				child.gameObject.AddComponent<Rigidbody>();
				child.GetComponent<Rigidbody>().mass= 2;
				FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
				joint.connectedBody = gameObject.GetComponent<Rigidbody>();
				joint.enableCollision = true;
				child.gameObject.GetComponent<FixedJoint>().breakForce = hV.body.brickBreakForce;
				child.gameObject.GetComponent<FixedJoint>().connectedAnchor = gameObject.GetComponent<Rigidbody>().position;
				child.gameObject.tag= "SingleMesh";
				//child.GetComponent<Rigidbody>().detectCollisions = false;
				//child.gameObject.AddComponent<DestroyBrick>();
			}
		}
		 
	 
	}


}