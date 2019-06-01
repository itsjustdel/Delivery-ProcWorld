using UnityEngine;
using System.Collections;

public class CreateBrickWall2 : ProcBase {
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine("Build");

	}

	IEnumerator Build()
	{

		HouseVariables hV;
		hV = GetComponent<HouseVariables>();
		//MeshBuilder meshBuilder = new MeshBuilder(); 
		Quaternion origRot = hV.body.lookRot;
		hV.body.lookRot = Quaternion.identity;

		Vector3 upDir = hV.bricks.brickSizeY*(hV.body.lookRot* Vector3.up);
		Vector3 rightDir =hV.bricks.brickSizeZ*(hV.body.lookRot*Vector3.right);
		Vector3 forwardDir = hV.bricks.brickSizeX*(hV.body.lookRot*Vector3.forward);
		//Vector3 offset = (-rightDir + forwardDir) * 0.5f + ((forwardDir/hV.body.bricksX)*1.5f) ;
		Vector3 offset = ((-Vector3.forward* hV.bricks.brickAmtX * hV.bricks.brickSizeX)*0.5f)+
						(-Vector3.right*hV.bricks.brickAmtZ * hV.bricks.brickSizeX)*0.5f;
	

		offset = hV.body.lookRot * offset;
		//offset += (-Vector3.right*hV.bricks.brickAmtZ * hV.bricks.brickSizeX)*0.5f;
		Vector3 farCorner = ((Vector3.forward* hV.bricks.brickAmtX* hV.bricks.brickSizeX)*0.5f + 
		                     (Vector3.right*hV.bricks.brickAmtZ*hV.bricks.brickSizeX)*0.5f);
		farCorner -= (hV.bricks.brickSizeX*0.5f * -  Vector3.forward);//* hV.bricks.brickSizeZ;


		farCorner = hV.body.lookRot * farCorner;

		float roofPeakCounter = hV.bricks.brickAmtZ;


		
		//Build 1st Row of "front"
		for (int i = 0; i < hV.bricks.brickAmtX; i++)
		{		
			offset += hV.bricks.brickSizeX* (hV.body.lookRot *  Vector3.forward);
			
			for (int j = 0; j < (hV.bricks.brickAmtY); j++)
			{
				//Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				Vector3 yOffset = (hV.bricks.brickSizeY* Vector3.up)*j*2;
				MeshBuilder meshBuilder = new MeshBuilder(); 

				BuildBrickWall2(meshBuilder,hV,offset,yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;				
				
				Vector3 center = offset + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (-forwardDir*0.5f) + (upDir*0.5f) + (rightDir*0.5f);
				center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
	//			BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();

				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
	//			meshCollider.size = new Vector3(rightDir.x,upDir.y,forwardDir.z);
	//			meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
	//			meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
	//			meshCollider.center = center;				//meshCollider.convex = true;

							
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

				yield return new WaitForFixedUpdate();
			}
		}
		//Build 2nd Row of "Front"
		for (int i = 0; i < hV.bricks.brickAmtX; i++)
		{
			
			offset -= (hV.bricks.brickSizeX* (hV.body.lookRot *  Vector3.forward));

			for (int j = 0; j < (hV.bricks.brickAmtY) - 1; j++)
			{
				//Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				Vector3 yOffset = (hV.bricks.brickSizeY* Vector3.up)*j*2;
				yOffset += (hV.bricks.brickSizeY* Vector3.up);

				MeshBuilder meshBuilder = new MeshBuilder(); 
				
				BuildBrickWall2(meshBuilder,hV  ,offset + ((hV.bricks.brickSizeX* (hV.body.lookRot* Vector3.forward))*0.5f),yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;				
				
				Vector3 center = offset + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (upDir*0.5f) + (rightDir*0.5f);
				center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;

		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
			//	MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(rightDir.x,upDir.y,forwardDir.z);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;	
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
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

				yield return new WaitForFixedUpdate();
			}
		}

		//Build 1st Row on Side

		for (int i = 0; i <  hV.bricks.brickAmtZ; i++)
		{			

			//offset -= (-rightDir)/hV.body.bricksX;
			offset -= (-hV.bricks.brickSizeX* (hV.body.lookRot * Vector3.right));
			
			for (int j = 0; j < (hV.bricks.brickAmtY-1); j++)
			{
				Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*j*2;
				yOffset += (hV.bricks.brickSizeY* Vector3.up);

			
				MeshBuilder meshBuilder = new MeshBuilder(); 
				//hV.body.lookRot = Quaternion.Euler(0,90,0);

				BuildBrickWall2(meshBuilder,hV,
				                offset - ((hV.body.lookRot*Vector3.right)*hV.bricks.brickSizeZ*0.5f) + (hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ*0.5f,
				                   yOffset);

				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				Vector3 center = offset + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (-forwardDir*0.25f) + (upDir*0.5f);// - (rightDir*1f);
						center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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

				yield return new WaitForFixedUpdate();

			}
		}

		//Build 2nd Row on Side
		
		for (int i = 0; i <  hV.bricks.brickAmtZ; i++)
		{			
			
			//offset -= (-rightDir)/hV.body.bricksX;
			offset += (-hV.bricks.brickSizeX* (hV.body.lookRot * Vector3.right));
			
			for (int j = 0; j < (hV.bricks.brickAmtY); j++)
			{
				Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*j*2;
				yOffset += (hV.bricks.brickSizeY* Vector3.up);
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
			//	hV.body.lookRot = Quaternion.Euler(0,90,0);
				BuildBrickWall2(meshBuilder,hV,
				                offset+ ((hV.bricks.brickSizeZ* (hV.body.lookRot*Vector3.forward))*0.5f) + ((hV.body.lookRot*Vector3.right)*hV.bricks.brickSizeZ*0.5f)
				                -Vector3.up*hV.bricks.brickSizeY,
				                   yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				
				//Vector3 center = offset + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				//center+= (-forwardDir*0.5f) + (upDir*0.5f) + (rightDir*0.5f);
				//center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;

				Vector3 center = offset + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center-= (upDir*0.5f) - (rightDir) + (forwardDir*0.25f);
						center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;

				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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
				
				yield return new WaitForFixedUpdate();
			}
		}
		//Back Wall

		for (int i = 0; i < hV.bricks.brickAmtX; i++)
		{		
			farCorner += hV.bricks.brickSizeX* - (hV.body.lookRot * Vector3.forward);
			
			for (int j = 0; j < (hV.bricks.brickAmtY); j++)
			{
				//Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				Vector3 yOffset = (hV.bricks.brickSizeY* Vector3.up)*j*2;
				MeshBuilder meshBuilder = new MeshBuilder(); 
				
				BuildBrickWall2(meshBuilder,hV,farCorner,// - ( hV.bricks.brickSizeX* -Vector3.forward)*0.5f ,
				                yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;				
				
				Vector3 center = farCorner + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (upDir*0.5f) + (rightDir*0.5f) -(forwardDir*0.5f);
				center+=((hV.body.brickSeperation*(hV.body.lookRot*Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(rightDir.x,upDir.y,forwardDir.z);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				
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
		
				yield return new WaitForFixedUpdate();
			}
		}

		for (int i = 0; i < hV.bricks.brickAmtX; i++)
		{
			
			farCorner += (hV.bricks.brickSizeX* (hV.body.lookRot * Vector3.forward));
			
			for (int j = 0; j < (hV.bricks.brickAmtY) - 1; j++)
			{
				//Vector3 yOffset =  (upDir/hV.body.bricksY)*j*2;
				Vector3 yOffset = (hV.bricks.brickSizeY* Vector3.up)*j*2;
				yOffset += (hV.bricks.brickSizeY* Vector3.up);
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				
				BuildBrickWall2(meshBuilder,hV  ,farCorner +( hV.bricks.brickSizeX* - (hV.body.lookRot*Vector3.forward))*0.5f ,yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;				
				
				Vector3 center = farCorner + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= -(forwardDir) + (upDir*0.5f) + (rightDir*0.5f);
						center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//	MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(rightDir.x,upDir.y,forwardDir.z);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;	
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
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
				yield return new WaitForFixedUpdate();
			}
		}

		for (int i = 0; i < hV.bricks.brickAmtZ; i++)
		{			
			
			//offset -= (-rightDir)/hV.body.bricksX;
			farCorner+= (-hV.bricks.brickSizeX* (hV.body.lookRot * Vector3.right));

			for (int j = 0; j < (hV.bricks.brickAmtY); j++)
			{
				Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*j*2;
				
				
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				//hV.body.lookRot = Quaternion.Euler(0,90,0);
				
				BuildBrickWall2(meshBuilder,hV,
				                farCorner + (Vector3.right*hV.bricks.brickSizeZ)*1.5f - (hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ*0.5f ,
				                yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				Vector3 center = farCorner + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (upDir*0.5f) - (forwardDir*0.75f) + rightDir*2;
								center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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
				
				yield return new WaitForFixedUpdate();
			}
		}

		for (int i = 0; i < hV.bricks.brickAmtZ; i++)
		{			
			
			//offset -= (-rightDir)/hV.body.bricksX;
			//offset += (-hV.bricks.brickSizeX*Vector3.right);
			farCorner-= (-hV.bricks.brickSizeX*(hV.body.lookRot * Vector3.right));

//			Vector3 offset2 = offset - ((hV.bricks.brickSizeX* hV.bricks.brickAmtX)*-Vector3.forward)+
//				((Vector3.right*hV.bricks.brickSizeX)*0.5f) + Vector3.forward*hV.bricks.brickSizeZ*0.5f;
			for (int j = 0; j < (hV.bricks.brickAmtY) - 1; j++)
			{
				Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*j*2;
				yOffset += (hV.bricks.brickSizeY* Vector3.up);
				
				
				MeshBuilder meshBuilder = new MeshBuilder(); 
				//hV.body.lookRot = Quaternion.Euler(0,90,0);
				
				BuildBrickWall2(meshBuilder,hV,
				                farCorner - (Vector3.right*hV.bricks.brickSizeZ)*1.5f - (hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ*0.5f,
				               yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
				Vector3 center = farCorner + yOffset;// - ((forwardDir/hV.body.bricksX)*0.5f) + ((upDir/hV.body.bricksY)*0.5f) - ((rightDir*hV.body.bricksDepth)*0.5f);
				center+= (upDir*0.5f)  -(rightDir) + (-forwardDir*0.75f); 
								center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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
				
				yield return new WaitForFixedUpdate();
			}
		}

		//Create "Triangles" for sides of house, 1nd Row

		for (int i = 0; i < hV.bricks.brickAmtZ; i++)
		{			
			Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*i;
			yOffset += ((hV.bricks.brickSizeY*hV.bricks.brickAmtY)* Vector3.up)*2;
			yOffset -= Vector3.up*hV.bricks.brickSizeY;

			for (int j = 0; j < roofPeakCounter ; j++)
			{

				farCorner -= (Vector3.right*hV.bricks.brickSizeX);
			

				MeshBuilder meshBuilder = new MeshBuilder(); 
				//hV.body.lookRot = Quaternion.Euler(0,90,0);
				
				BuildBrickWall2(meshBuilder,hV,
						                farCorner + (Vector3.right*hV.bricks.brickSizeX)*0.5f - (hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ*0.5f,
				                yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
						Vector3 center =  farCorner + (Vector3.right*hV.bricks.brickSizeX)*0.5f - (hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ*0.5f + yOffset;
				center+= (upDir*0.5f) +(rightDir*0.5f) + (-forwardDir*0.5f); 
								center+=((hV.body.brickSeperation*(hV.body.lookRot* Vector3.forward)) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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
				
				yield return new WaitForFixedUpdate();
			}
			roofPeakCounter-=1;
			farCorner+= (Vector3.right*hV.bricks.brickSizeX)*roofPeakCounter;
			farCorner+= (Vector3.right*hV.bricks.brickSizeX)*0.5f;
		}



		roofPeakCounter = hV.bricks.brickAmtZ;

		for (int i = 0; i < hV.bricks.brickAmtZ; i++)
		{			
			Vector3 yOffset =  Vector3.up*hV.bricks.brickSizeY*i;
			yOffset += ((hV.bricks.brickSizeY*hV.bricks.brickAmtY)* Vector3.up)*2;
			yOffset -= Vector3.up*hV.bricks.brickSizeY;
			
			for (int j = 0; j < roofPeakCounter ; j++)
			{
				
				//offset -= (Vector3.right*hV.bricks.brickSizeX);				
				offset += (hV.bricks.brickSizeX*Vector3.right); 

				MeshBuilder meshBuilder = new MeshBuilder(); 
				//hV.body.lookRot = Quaternion.Euler(0,90,0);
				
				BuildBrickWall2(meshBuilder,hV,
						                offset - (Vector3.right*hV.bricks.brickSizeX)*0.5f + ((hV.body.lookRot*Vector3.forward)*hV.bricks.brickSizeZ)*0.5f,
				                yOffset);
				
				GameObject child = new GameObject();
				child.name = "Brick";
				child.transform.position = this.gameObject.transform.position;
				child.transform.parent = this.gameObject.transform;
				
						Vector3 center = offset - (Vector3.right*hV.bricks.brickSizeX)*0.5f +(hV.body.lookRot* Vector3.forward)*hV.bricks.brickSizeZ*0.5f + yOffset;
				center+= (upDir*0.5f) +(rightDir*0.5f) + (-forwardDir*0.5f); 
				center+=((hV.body.brickSeperation*Vector3.forward) - hV.body.brickSeperation*Vector3.up)*0.5f;
				
				
		//		BoxCollider meshCollider = child.gameObject.AddComponent<BoxCollider>();
				MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();	//meshCollider.size = new Vector3(rightDir.x*hV.body.bricksDepth,upDir.y/hV.body.bricksY,forwardDir.z/hV.body.bricksX);
		//		meshCollider.size = new Vector3(hV.bricks.brickSizeZ,hV.bricks.brickSizeY,hV.bricks.brickSizeX);
		//		meshCollider.size -=(hV.body.brickSeperation*Vector3.forward);
		//		meshCollider.size -= (hV.body.brickSeperation*Vector3.up);				
		//		meshCollider.center = center;				//meshCollider.convex = true;
				
				//MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
				//MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();					
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
				
				
				child.transform.RotateAround(meshRenderer.bounds.center,Vector3.up,(Random.Range(-5.0f,5.0f))+90);
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
				
				yield return new WaitForFixedUpdate();
			}
			roofPeakCounter-=1;
			offset -= (Vector3.right*hV.bricks.brickSizeX)*roofPeakCounter;
			offset -= (Vector3.right*hV.bricks.brickSizeX)*0.5f;
		}

		gameObject.GetComponent<MeshControl>().enabled = true;

		transform.rotation = origRot * Quaternion.Euler(0,90,0);

	}
}