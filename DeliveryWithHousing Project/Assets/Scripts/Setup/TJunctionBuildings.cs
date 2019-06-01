using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TJunctionBuildings : ProcBase {

	public int houseAmt = 10;

	public BRoadMesher bRoadMesher;
	public Transform house;

	public int startInd;
	public int endInd;
	public Mesh startMesh;
	public Mesh endMesh;

	public Vector3 middle;
	//kerbs
	public Transform kerbPrefab;
	public int pavementLength  = 100;
	private float kerbFrequency = 100f;
	public float kerbSizeX = 0.5f;
	public float kerbSizeY = 0.1f;
	public int pavementDepth = 10;
	private int xSize = 300;

	//private float frequencyPavement = 50f;
	private float yScaler = 0.1f;

	public bool placeHouses;

	public HouseVariables hV;
	// Use this for initialization
	void Start () {


		//grab stuff we need from the cell we attched this scrip to
		startInd = transform.GetComponent<RingRoadList>().startInd;
		//startMesh = transform.FindChild("RingRoad").GetComponent<MeshFilter>().mesh;
		startMesh = transform.GetComponent<RingRoadList>().startMesh;
		endInd = transform.GetComponent<RingRoadList>().endInd;
		endMesh = transform.GetComponent<RingRoadList>().endMesh;


		//StartCoroutine("PlaceTrigger");
	//	StartCoroutine("Kerbs");
		StartCoroutine("StartPlacing");
	//	StartCoroutine("AddBorderToVoronoi");
	//	StartCoroutine("AddRoadToVoronoi");
		//invert junction mesh array


	}
	void Update()
	{
		if(placeHouses)
		{
			StartCoroutine("StartPlacing");
			placeHouses = false;
		}
	}

	IEnumerator AddRoadToVoronoi()
	{
		MeshGenerator mG = transform.GetComponent<MeshGenerator>();
		int numberOfPoints = 1000;

		for (int i = 0; i < numberOfPoints; i+=50)
		{
			Vector3 pos = Vector3.Lerp(startMesh.vertices[startInd + i - (numberOfPoints/2)],
			                           startMesh.vertices[startInd + i - (numberOfPoints/2) +1],
			                           0.5f);
			pos.y = 0;
			mG.yardPoints.Add(pos);
		}

		yield break;
	}


	IEnumerator AddBorderToVoronoi()
	{
		//pass the centre of the tjunction to the circle function
		Vector2 center = new Vector2(startMesh.vertices[startInd].x,startMesh.vertices[startInd].z);


		DrawCirclePoints(20,200,center);
		DrawCirclePoints(20,100,center);
		yield break;
	}

	void DrawCirclePoints(int points, double radius, Vector2 center)
	{

		MeshGenerator mG = transform.GetComponent<MeshGenerator>();

		float slice = 2 * Mathf.PI / points;
		for (int i = 0; i < points; i++)
		{
			float angle = slice * i;
			int newX = (int)(center.x + radius * Mathf.Cos(angle));
			int newY = (int)(center.y + radius * Mathf.Sin(angle));
			Vector2 p = new Vector2(newX, newY);
		
			Vector3 p3 = new Vector3(p.x,0f,p.y);

			mG.yardPoints.Add(p3);
		}
	}


	IEnumerator Kerbs()
	{
		StartCoroutine("KerbAcrossFromJunction");
		StartCoroutine("KerbThatGoesThroughJunction");
		StartCoroutine("KerbAlongBRoad");


		yield break;
	}



	IEnumerator KerbAlongBRoad()
	{
		//Add the the end of the B Road to two curves. One for each side of the road.

		Mesh bRoadMesh = transform.Find("BRoad").GetComponent<MeshFilter>().mesh;
		Mesh junctionMeshOrig = transform.Find("Junction Section").GetComponent<MeshFilter>().mesh;
		Mesh junctionMesh = junctionMeshOrig;



		if (startInd %2 !=0)
		{
			BezierSpline splineL = gameObject.AddComponent<BezierSpline>();
			BezierSpline splineR = gameObject.AddComponent<BezierSpline>();
			List<Vector3> tempListL = new List<Vector3>();
			List<BezierControlPointMode> tempControlListL = new List<BezierControlPointMode>();
			List<Vector3> tempListR = new List<Vector3>();
			List<BezierControlPointMode> tempControlListR = new List<BezierControlPointMode>();
			//find closest point on BRoad mesh to the start of the junction
			
			int startIndBRoad = 0;
			
			//Mesh targetRoadMesh = gameObject.transform.FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;
			
			//Find the closest point on the BRoad from the end of Junction Mesh
			float minDistanceSqr = Mathf.Infinity;
			
			for (int i = 0; i < bRoadMesh.vertices.Length ; i++)
			{
				Vector3 midPoint = junctionMesh.vertices[junctionMesh.vertexCount/2];				
				Vector3 diff = midPoint-bRoadMesh.vertices[i];
				float distSqr = diff.sqrMagnitude;
				
				if (distSqr < minDistanceSqr)
				{
					minDistanceSqr = distSqr;
					startIndBRoad = i ;					
				}			
			}
			
			//Now we have the closest Ind, create a curve from this point to the of the BRoad
			//startInd +=1;
			for (int i = 0; i < startIndBRoad; i+=2)
			{
				tempListL.Add(bRoadMesh.vertices[ startIndBRoad - i + 1 ] );
				tempControlListL.Add(BezierControlPointMode.Aligned);
			}

			//Use the junction Mesh for the other side of the road
			//Start from half way up the Junction Mesh
			for (int i = junctionMesh.vertexCount/2; i <junctionMesh.vertexCount; i+=2)
			{

				tempListR.Add(junctionMesh.vertices[ i ]);
				tempControlListR.Add(BezierControlPointMode.Aligned);
			}


			splineL.points = tempListL.ToArray();
			splineL.modes =  tempControlListL.ToArray();
			splineR.points = tempListR.ToArray();
			splineR.modes =  tempControlListR.ToArray();			
			
			StartCoroutine(CreateKerb(splineL,"BRoad"));
			StartCoroutine(CreateKerb(splineR,"Junction"));

			yield return new WaitForFixedUpdate();

			StartCoroutine(CreatePavement(splineL, "left",false));
			StartCoroutine(CreatePavement(splineR, "right",false));

		}

		if (startInd %2 ==0) //If b Road is joining on to the A Road at the even side of its indices
		{

			BezierSpline splineL = gameObject.AddComponent<BezierSpline>();
			BezierSpline splineR = gameObject.AddComponent<BezierSpline>();
			List<Vector3> tempListL = new List<Vector3>();
			List<BezierControlPointMode> tempControlListL = new List<BezierControlPointMode>();
			List<Vector3> tempListR = new List<Vector3>();
			List<BezierControlPointMode> tempControlListR = new List<BezierControlPointMode>();
			//find closest point on BRoad mesh to the start of the junction
			
			int startIndBRoad = 0;
			float minDistanceSqr = Mathf.Infinity;

			//Find the nearerst point on the B Roadmesh from half way up the junction
			for (int i = 0; i < bRoadMesh.vertices.Length; i++)
			{
				Vector3 midPoint = junctionMesh.vertices[junctionMesh.vertexCount/2];				
				Vector3 diff = midPoint-bRoadMesh.vertices[i];
				float distSqr = diff.sqrMagnitude;
				
				if (distSqr < minDistanceSqr)
				{
					minDistanceSqr = distSqr;
					startIndBRoad = i ;
					
				}			
			}

			//Now we have the closest Ind, create a curve from this point to the of the BRoad
			for (int i = 0; i < startIndBRoad ; i+=2)
			{
				tempListL.Add(bRoadMesh.vertices[ startIndBRoad - i ] );
				tempControlListL.Add(BezierControlPointMode.Aligned);

			}

			//Use the junction Mesh for the other side of the road
			for (int i = 1; i <(junctionMesh.vertexCount/2)-1; i+=2)
			{				
				tempListR.Add(junctionMesh.vertices[ junctionMesh.vertices.Length - i  ]);
				tempControlListR.Add(BezierControlPointMode.Aligned);
			}
			splineL.points = tempListL.ToArray();
			splineL.modes =  tempControlListL.ToArray();
			splineR.points = tempListR.ToArray();
			splineR.modes =  tempControlListR.ToArray();
			
			StartCoroutine(CreateKerb(splineL,"BRoad"));
			StartCoroutine(CreateKerb(splineR,"Junction"));
			
			yield return new WaitForFixedUpdate();
			StartCoroutine(CreatePavement(splineL, "right",false));
			StartCoroutine(CreatePavement(splineR, "right",false));
		}



		yield break;
	}

	IEnumerator KerbAcrossFromJunction()
	{
		
		BezierSpline spline = gameObject.AddComponent<BezierSpline>();
		List<Vector3> tempList = new List<Vector3>();
		List<BezierControlPointMode> tempControlList = new List<BezierControlPointMode>();
		//Add points across from the junction to a Bezier
		for (int i = 0; i < pavementLength; i+=2)
		{
		
			tempList.Add(startMesh.vertices[startInd+1+i - (pavementLength/2)]);
			tempControlList.Add(BezierControlPointMode.Aligned);
		}
		
		spline.points = tempList.ToArray();
		spline.modes = tempControlList.ToArray();
		
		StartCoroutine(CreateKerb(spline,"Across"));

		yield return new WaitForFixedUpdate();
		
		if (startInd %2 ==0)
			StartCoroutine(CreatePavement(spline,"left",false));

		if (startInd %2 !=0)
			StartCoroutine(CreatePavement(spline,"right",false));

		yield break;
	}

	IEnumerator KerbThatGoesThroughJunction()
	{
		BezierSpline spline = gameObject.AddComponent<BezierSpline>();
		List<Vector3> tempList = new List<Vector3>();
		List<BezierControlPointMode> tempControlList = new List<BezierControlPointMode>();
		//Add points across from the junction to a Bezier
		for (int i = 0; i < pavementLength; i+=2)
		{
			tempList.Add(startMesh.vertices[startInd+i - (pavementLength/2)]);
			tempControlList.Add(BezierControlPointMode.Aligned);
		}
		
		spline.points = tempList.ToArray();
		spline.modes = tempControlList.ToArray();
		
		StartCoroutine(CreateKerb(spline,"Through"));

		yield return new WaitForFixedUpdate();

		if (startInd %2 ==0)
			StartCoroutine(CreatePavement(spline,"right",true));
		
		if (startInd %2 !=0)
			StartCoroutine(CreatePavement(spline,"left",true));
		
		yield break;
	}

	IEnumerator CreatePavement(BezierSpline bezier, string side, bool through)
	{
		//meshFilter = plane.gameObject.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		List<Vector3> tempList = new List<Vector3>();
			
		
		for (int r = 0; r <=  pavementDepth ;r++)//how deep each section is
		{
			for (float i = 0; i <=xSize;i++)//how long each section is// Change += to change the density level
			{
				
				Vector3 position = bezier.GetPoint((i)/(xSize-1));
				Vector3 direction = bezier.GetDirection (i/(xSize-1));
				//				float random = Random.Range(0.5f,1.5f);
				Vector3 pAndR = Vector3.zero;
				Vector3 rot1 = Vector3.zero;
				//Check what side of the road it has been passed and adjust directional vector accordingly

				if (side == "left")
				{
					rot1 = Quaternion.Euler (0, 90, 0) * (direction*r*yScaler);  //this direction should be found with startInd - startInd+1??
					//rot1 = Vector3.right*r;
				} 
				else if (side == "right")
				{
					rot1 = Quaternion.Euler (0, -90, 0) * (direction*r*yScaler);
					//rot1 = Vector3.left*r;
				}		

				Vector3 kerbHeight = kerbSizeY*0.5f*Vector3.up* 0.8f; //0.8f brings it just under the lip of the kerbstone

				Vector3 target = position + kerbHeight + rot1 - transform.position;
				RaycastHit hit;

				//LayerMask lM = LayerMask.GetMask("Kerb");

				//if (through)
				LayerMask	lM = LayerMask.GetMask("Kerb","Road");

				if(Physics.Raycast(target,Vector3.down*10,out hit,20f,lM))
				{

					target = hit.point - Vector3.up*0.5f;
				}	

				tempList.Add(target);

		//		Transform item = Instantiate(bRoadMesher.cube) as Transform;
		//		item.position = position +transform.position + rot1;
		//		item.name = "pavement cube";
			}
			
		}
		
		
		int[] triangles = new int[(xSize * pavementDepth) * 6];

		
		if (side == "right")
		{
			for (int ti = 0, vi = 0, y = 0; y < pavementDepth; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}
			}
		}
		
		if (side == "left")
		{
			for (int ti = 0, vi = 0, y = 0; y < pavementDepth; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 4] = triangles[ti + 1] = vi + 1;
					triangles[ti + 3] = triangles[ti + 2] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}
			}
		}	
		mesh.vertices = tempList.ToArray();

		mesh.triangles = triangles;
		
		
		mesh.RecalculateNormals();
		;
		
		GameObject verge = new GameObject();
		verge.transform.parent = this.gameObject.transform;
		verge.name = "Pavement";
		verge.layer = 9;

	//	verge.tag = "Pavement";
		//verge.layer = 10;
		MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
		meshFilterInstance.mesh = mesh;
		MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load("Grey",typeof(Material)) as Material;
		MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	
		
		yield break;

	}

	IEnumerator CreateKerb(BezierSpline spline, string type)
	{
		float frequency = kerbFrequency;		

		float stepSize = frequency;
		stepSize = 1f / (frequency -1);
		
		LayerMask lM = LayerMask.GetMask("Road");
		RaycastHit hit;
		for (int f = 0; f <= frequency; f++) { 
			
			Vector3 position = spline.GetPoint(f * stepSize)-transform.position;
			//RayCast down and skip if over a junction
	


			Vector3 nextPos = spline.GetPoint((f+1) * stepSize)-transform.position;
			//nextPoint is used to determine where the Quad is stretched to
			Vector3 nextPoint = nextPos-position;
			nextPoint *=0.98f;//makes the kerbs seperate instead of a big snake
			
			Vector3 direction = spline.GetDirection(f*stepSize);
			Quaternion lookRot = Quaternion.LookRotation (direction);
			Vector3 right = lookRot * Vector3.right * kerbSizeX;
			
			Vector3 kerbHeight = kerbSizeY*(lookRot*Vector3.up);
			
			MeshBuilder meshBuilder = new MeshBuilder();
			if( type == "Through")
			{
				if (startInd %2 ==0) //If b Road is joining on to the A Road at the even side of its indices
				{

					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,-90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						    continue;	
				}
				else if (startInd %2 !=0) // If b Road is joining on to the A Road at the odd side of its indices
				{
					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						continue;
				}
			
			}

			if( type == "BRoad")
			{	
				if (startInd %2 ==0) //If b Road is joining on to the A Road at the even side of its indices
				{
					
					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,-90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						continue;	
				}
				else if (startInd %2 !=0) // If b Road is joining on to the A Road at the odd side of its indices
				{
					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						continue;
				}					
			}

			if( type == "Junction")
			{
				if (startInd %2 ==0) //If b Road is joining on to the A Road at the even side of its indices
				{
					
					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,-90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						continue;	
				}
				else if (startInd %2 !=0) // If b Road is joining on to the A Road at the odd side of its indices
				{
					Vector3 raypos = position +  Quaternion.Euler(new Vector3(0,-90,0))*direction*0.1f;					
					if (Physics.Raycast(raypos + (Vector3.up*10), -Vector3.up, out hit, 20.0F,lM))
						continue;
				}	
			}
			//move the offsest half a size back and down
			position -= nextPoint*0.5f;
			position -= right*0.5f;
			position -= kerbHeight*0.5f;

			BuildQuad(meshBuilder,position,nextPoint,right);//bottom
			BuildQuad(meshBuilder,position,right,kerbHeight);
			BuildQuad(meshBuilder,position,kerbHeight,nextPoint);//far side
			
			Vector3 farCorner = kerbHeight+nextPoint + position;
			
			BuildQuad(meshBuilder,farCorner,-nextPoint,right);//top
			BuildQuad(meshBuilder,farCorner,right,-kerbHeight);
			BuildQuad(meshBuilder,farCorner+right,-nextPoint,-kerbHeight);
			
			GameObject child = new GameObject();
			MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = child.AddComponent<MeshFilter>();
			MeshCollider meshCollider = child.AddComponent<MeshCollider>();
			child.transform.parent = this.gameObject.transform;
			child.layer = 16;
			child.name = "Kerb";
			
			//creates a rotation for each kerb, so they look wonky. The 2f could be replaces for a variable
			child.transform.RotateAround(position,Vector3.up,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.right,UnityEngine.Random.Range(-0.2f,0.2f));
			child.transform.RotateAround(position,Vector3.forward,UnityEngine.Random.Range(-0.2f,0.2f));
			
			Mesh mesh = meshBuilder.CreateMesh();
			meshFilter.sharedMesh = mesh;
			meshCollider.sharedMesh = mesh;
			
			meshRenderer.sharedMaterial = Resources.Load("Grey",typeof(Material)) as Material;
		}
		yield break;
	}

	/// <summary>
	/// Instantiate Building off to the side of A Road 
	/// </summary>
	IEnumerator PlaceBuilding()
	{
		//yield return new WaitForSeconds(1);

		int randomNumber = UnityEngine.Random.Range(-100,100);
		int newInd = startInd + randomNumber;



		//create direction away from the road using the point found by the b Road for the connection
		//and the point on the other side of the A Road Mesh
		if (newInd > startMesh.vertexCount)
			newInd -= startMesh.vertexCount;


		 middle = Vector3.Lerp(startMesh.vertices[newInd],startMesh.vertices[newInd+1],0.5f);

		Vector3 direction = startMesh.vertices[newInd] - startMesh.vertices[newInd+1];
		direction.Normalize();

	//	float randomNumber2 =UnityEngine.Random.Range(10f,20f);
		direction*=7;

		//Create position to place building

		Vector3 position = middle + direction;
			
		//Place the prefab at this newly created position
		house = Resources.Load("Prefabs/Setup/House No Road", typeof (Transform)) as Transform;
		Transform item = Instantiate(house) as Transform;
		item.name = "Test House";
		item.position = position;
		item.parent = this.transform;

		//tell this sscript this mesh indice position so that if the building does not get destroyed on setup
		//it can pass its position (mesh and vertice position) to the voronoi mesh script
		//rotation
		item.Find("Cube").GetComponent<ColliderCheck>().ind = newInd;
		item.Find("Cube").GetComponent<ColliderCheck>().cell = item.gameObject;

		Vector3 lookPos = startMesh.vertices[newInd+1] - item.transform.position;
		lookPos.y = 0;
		Quaternion rotation = Quaternion.LookRotation(lookPos);
		HouseVariables cb2 =  item.Find("Brick House").GetComponent<HouseVariables>();
		cb2.body.lookRot = rotation;

		item.transform.Find("Cube").rotation = rotation;
		//height
		RaycastHit hit;		
		LayerMask myLayerMask = LayerMask.GetMask("Road");		
		if (Physics.Raycast(item.transform.position + Vector3.up*100, -Vector3.up, out hit, 200.0F,myLayerMask))
		{
			item.transform.position = hit.point;
			Debug.Log("HIT!!");

		//	Transform item2 = Instantiate(bRoadMesher.cube) as Transform;
		//	item2.position = hit.point;
		//			item2.name = "hit cube";
		}
		item.transform.Find("Cube").position += Vector3.up * item.transform.Find("Cube").localScale.y * 0.5f;

		yield break;
	}

	/// <summary>
	/// Place a Trigger to prebuild the junction area regardless of van whereabouts
	/// </summary>
	IEnumerator PlaceTrigger()
	{
		GameObject boxColliderGo = new GameObject();
		boxColliderGo.transform.parent = this.transform;
		boxColliderGo.AddComponent<ActivateObjects>();
		boxColliderGo.name = "T Junction Collider";

		BoxCollider boxCollider =  boxColliderGo.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size *= 200;

		boxColliderGo.transform.position = startMesh.vertices[startInd];
		//boxColliderGo.SetActive(false);
		yield break;
	}

	IEnumerator StartPlacing()
	{
		for (int i = 0; i < houseAmt; i++)
		{
			StartCoroutine("PlaceBuilding");
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}




}
